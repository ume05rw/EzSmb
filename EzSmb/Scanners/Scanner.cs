using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EzSmb.Scanners
{
    internal class Scanner : IDisposable
    {
        private static readonly byte[] NameQueryBytes = new byte[]
        {
            0x04, 0xa9, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x20, 0x43, 0x4b,
            0x41, 0x41, 0x41, 0x41, 0x41,
            0x41, 0x41, 0x41, 0x41, 0x41,
            0x41, 0x41, 0x41, 0x41, 0x41,
            0x41, 0x41, 0x41, 0x41, 0x41,
            0x41, 0x41, 0x41, 0x41, 0x41,
            0x41, 0x41, 0x41, 0x41, 0x41,
            0x00, 0x00, 0x21, 0x00, 0x01,
        };

        private List<ClientSet> _clients;
        private Dictionary<IPAddress, IPAddress[]> _querySet;
        private List<IPAddress> _resultAddresses;

        private bool disposedValue;

        public Scanner()
        {
            this._clients = new List<ClientSet>();
            this._querySet = new Dictionary<IPAddress, IPAddress[]>();
            this._resultAddresses = new List<IPAddress>();

            this.InitTargets();
        }

        private void InitTargets()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var nic in nics)
            {
                if (
                    nic.OperationalStatus != OperationalStatus.Up
                    || nic.NetworkInterfaceType == NetworkInterfaceType.Loopback
                    || nic.NetworkInterfaceType == NetworkInterfaceType.Tunnel
                )
                {
                    continue;
                }

                var props = nic.GetIPProperties();
                if (props == null)
                    continue;

                foreach (var uAddr in props.UnicastAddresses)
                {
                    if (
                        uAddr.Address.AddressFamily != AddressFamily.InterNetwork
                        || IPAddress.IsLoopback(uAddr.Address)
                    )
                    {
                        continue;
                    }

                    var addrStr = uAddr.Address.ToString();
                    if (
                        !addrStr.StartsWith("10.")
                        && !addrStr.StartsWith("192.168.")
                    )
                    {
                        var found = false;
                        for (var i = 16; i <= 31; i++)
                        {
                            if (addrStr.StartsWith($"172.{i}."))
                                found = true;
                        }

                        if (!found)
                            // Not private address.
                            continue;
                    }

                    var addrBytes = uAddr.Address.GetAddressBytes();
                    var maskBytes = uAddr.IPv4Mask.GetAddressBytes();
                    var addrBits = new BitArray(addrBytes);
                    var maskBits = new BitArray(maskBytes);
                    var beginBits = new BitArray(32);
                    var endBits = new BitArray(32);

                    for(var i = 0; i < addrBits.Length; i++)
                    {
                        if (maskBits[i])
                        {
                            beginBits[i] = addrBits[i];
                            endBits[i] = addrBits[i];
                        }
                        else
                        {
                            beginBits[i] = false;
                            endBits[i] = true;
                        }
                    }

                    var beginBytes = new byte[4];
                    var endBytes = new byte[4];
                    beginBits.CopyTo(beginBytes, 0);
                    endBits.CopyTo(endBytes, 0);

                    Array.Reverse(beginBytes);
                    Array.Reverse(endBytes);

                    var beginUint = BitConverter.ToUInt32(beginBytes, 0);
                    var endUint = BitConverter.ToUInt32(endBytes, 0);

                    if (1024 < (endUint - beginUint + 1))
                        // Too wide address range.
                        continue;

                    var addresses = new List<IPAddress>();

                    for (var i = beginUint + 1; i < endUint; i++)
                    {
                        var queryToBytes = BitConverter.GetBytes(i);
                        Array.Reverse(queryToBytes);
                        addresses.Add(new IPAddress(queryToBytes));
                    }

                    this._querySet.Add(uAddr.Address, addresses.ToArray());
                }
            }
        }

        public async Task<IPAddress[]> Scan(int waitMsec = 0)
        {
            foreach (var pair in this._querySet)
            {
                var cset = new ClientSet(pair.Key);

                cset.Client.BeginReceive(this.OnRecieved, cset);

                foreach (var toAddr in pair.Value)
                {
                    var destEp = new IPEndPoint(toAddr, 137);
                    cset.Client.Send(
                        Scanner.NameQueryBytes,
                        Scanner.NameQueryBytes.Length,
                        destEp
                    );
                }

                this._clients.Add(cset);
            }

            var wait = (0 < waitMsec)
                ? (int)waitMsec
                : 3000;

            await Task.Delay(wait)
                .ConfigureAwait(false);

            return this._resultAddresses.ToArray();
        }

        /// <summary>
        /// Event handling at data reception.
        /// </summary>
        /// <param name="ar"></param>
        private void OnRecieved(IAsyncResult ar)
        {
            if (this.disposedValue || ar == null || ar.AsyncState == null)
                return;

            // Get UdpClient.
            var cset = (ClientSet)ar.AsyncState;
            var endPoint = default(IPEndPoint);

            try
            {
                var bytes = cset.Client.EndReceive(ar, ref endPoint);
                if (bytes != null && 0 < bytes.Length)
                    this._resultAddresses.Add(endPoint.Address);
            }
            catch (Exception)
            {
                // Recieve-Socket closed or disposed.
                return;
            }

            try
            {
                cset.Client.BeginReceive(this.OnRecieved, cset);
            }
            catch (Exception)
            {
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (this._clients != null)
                    {
                        foreach (var client in this._clients)
                        {
                            try
                            {
                                client?.Dispose();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    this._clients?.Clear();
                    this._querySet?.Clear();
                    this._resultAddresses?.Clear();

                    this._clients = null;
                    this._querySet = null;
                    this._resultAddresses = null;
                }

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}


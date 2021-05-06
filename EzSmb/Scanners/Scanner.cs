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

        private List<Socket> _sockets;
        private Dictionary<IPAddress, IPAddress[]> _querySet;
        private List<IPAddress> _resultAddresses;

        private bool disposedValue;

        public Scanner()
        {
            this._sockets = new List<Socket>();
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
                var localEp = new IPEndPoint(pair.Key, 0);

                foreach (var toAddr in pair.Value)
                {
                    var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                    socket.Bind(localEp);

                    var sset = new SocketSet(socket, toAddr);
                    var endPoint = new IPEndPoint(IPAddress.Any, 0) as EndPoint;

                    socket.BeginReceiveMessageFrom(
                        sset.ReceiveBuffer,
                        0,
                        sset.ReceiveBuffer.Length,
                        SocketFlags.None,
                        ref endPoint,
                        this.OnRecieved,
                        sset
                    );

                    var destEp = new IPEndPoint(toAddr, 137);
                    socket.SetSocketOption(
                        SocketOptionLevel.Socket,
                        SocketOptionName.Broadcast,
                        false
                    );
                    socket.SendTo(Scanner.NameQueryBytes, destEp);

                    this._sockets.Add(socket);
                }
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
            if (this.disposedValue)
                return;

            // Get SocketSet-object.
            var sset = (SocketSet)ar.AsyncState;
            var sFlags = SocketFlags.None;
            var endPoint = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

            int length;
            try
            {
                length = sset.Socket.EndReceiveMessageFrom(
                    ar,
                    ref sFlags,
                    ref endPoint,
                    out IPPacketInformation pInfo
                );

                if (0 < length)
                    this._resultAddresses.Add(sset.RemoteAddress);
            }
            catch (Exception)
            {
                // Recieve-Socket closed or disposed.
                return;
            }
            finally
            {
                sset?.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (this._sockets != null)
                    {
                        foreach (var socket in this._sockets)
                        {
                            try
                            {
                                socket?.Close();
                            }
                            catch (Exception)
                            {
                            }

                            try
                            {
                                socket?.Dispose();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    this._querySet?.Clear();
                    this._resultAddresses?.Clear();

                    this._sockets = null;
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


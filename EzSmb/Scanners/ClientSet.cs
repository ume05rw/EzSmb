using System;
using System.Net;
using System.Net.Sockets;

namespace EzSmb.Scanners
{
    internal class ClientSet : IDisposable
    {
        private const int BufferSize = 2048;

        public UdpClient Client;
        public byte[] ReceiveBuffer;

        public ClientSet(IPAddress localAddress)
        {
            var localEp = new IPEndPoint(localAddress, 0);
            this.Client = new UdpClient(localEp);
            this.ReceiveBuffer = new byte[BufferSize];
        }

        #region IDisposable Support
        public bool IsDisposed { get; private set; } = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (disposing)
                {
                    if (this.Client != null)
                    {
                        try
                        {
                            this.Client?.Close();
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            this.Client?.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    this.Client = null;
                    this.ReceiveBuffer = null;
                }

                this.IsDisposed = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

}

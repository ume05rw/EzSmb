using System;
using System.Net;
using System.Net.Sockets;

namespace EzSmb.Scanners
{
    internal class SocketSet : IDisposable
    {
        private const int BufferSize = 2048;

        public Socket Socket;
        public byte[] ReceiveBuffer;
        public IPAddress RemoteAddress;

        public SocketSet(Socket soc, IPAddress remoteAddr)
        {
            this.Socket = soc;
            this.RemoteAddress = remoteAddr;
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
                    if (this.Socket != null)
                    {
                        try
                        {
                            this.Socket?.Close();
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            this.Socket?.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    this.RemoteAddress = null;
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

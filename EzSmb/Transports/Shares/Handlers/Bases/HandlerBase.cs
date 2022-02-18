using EzSmb.Params.Enums;
using EzSmb.Shareds.Bases;
using EzSmb.Transports.Shares.Handlers.Enums;
using EzSmb.Transports.Shares.Handlers.Interfaces;
using SMBLibrary;
using SMBLibrary.Client;
using System;

namespace EzSmb.Transports.Shares.Handlers.Bases
{
    /// <summary>
    /// Handle object manager
    /// </summary>
    /// <remarks>
    /// for using statement.
    /// </remarks>
    internal abstract class HandlerBase : ErrorManagedBase, IHandler
    {
        private ISMBFileStore _store;
        private bool disposedValue;

        public object Handle { get; private set; }
        public FileStatus FileStatus { get; private set; }
        public bool Succeeded { get; private set; }
        public NTStatus Status { get; private set; }

        public HandlerBase(
            SmbType smbType,
            ISMBFileStore store,
            string path,
            HandleType handleType,
            NodeType nodeType
        )
        {
            this.Handle = null;
            this.Succeeded = false;

            if (store == null || nodeType == NodeType.Server)
                return;

            this._store = store;
            var args = handleType.ToArgs(smbType, nodeType);
            Status = this._store.CreateFile(
                out var handle,
                out var handleStatus,
                path,
                args.AccessMask,
                args.FileAttributes,
                args.ShareAccess,
                args.CreateDisposition,
                args.CreateOptions,
                null
            );

            if (Status == NTStatus.STATUS_SUCCESS)
            {
                this.Handle = handle;
                this.FileStatus = handleStatus;
                this.Succeeded = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (this.Handle != null)
                    {
                        try
                        {
                            this._store?.CloseFile(this.Handle);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    this._store = null;
                    this.Handle = null;
                }

                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}

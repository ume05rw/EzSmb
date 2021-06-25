using EzSmb.Params;
using EzSmb.Params.Enums;
using EzSmb.Paths;
using EzSmb.Shareds;
using EzSmb.Shareds.Bases;
using EzSmb.Transports.Shares.Factories;
using EzSmb.Transports.Shares.Interfaces;
using SMBLibrary;
using SMBLibrary.Client;
using System;
using System.Linq;

namespace EzSmb.Transports
{
    /// <summary>
    /// ISMBClient manager class
    /// </summary>
    internal class Connection : ErrorManagedBase
    {
        private PathSet _pathSet;
        private FixedParamSet _paramSet;
        private ISMBClient _client;
        private bool disposedValue;

        public bool IsConnected { get; }

        public Connection(PathSet pathSet, ParamSet paramSet) : base()
        {
            this.IsConnected = false;

            if (pathSet == null)
            {
                this.AddError("Constructor", "Required pathSet.");

                return;
            }

            this._pathSet = pathSet.Clone();
            var argParamSet = (paramSet == null)
                ? new ParamSet()
                : paramSet.Clone();

            if (argParamSet.SmbType == null)
            {
                this._client
                    = this.GetConnection(SmbType.Smb2)
                    ?? this.GetConnection(SmbType.Smb1);
            }
            else if (argParamSet.SmbType == SmbType.Smb2)
            {
                this._client = this.GetConnection(SmbType.Smb2);
            }
            else if (argParamSet.SmbType == SmbType.Smb1)
            {
                this._client = this.GetConnection(SmbType.Smb1);
            }
            else
            {
                this.AddError("Constructor", $"Unexpected ParamSet.SmbType: {argParamSet.SmbType}");

                return;
            }

            if (this._client == null)
            {
                this.AddError("Constructor", "Connection Failed.");

                return;
            }

            if (!this.Login(argParamSet))
            {
                this.AddError("Constructor", "Authentication Failed.");

                return;
            }

            this._paramSet = FixedParamSet.Parse(
                argParamSet,
                (this._client is SMB2Client)
                    ? SmbType.Smb2
                    : SmbType.Smb1
            );

            this.IsConnected = true;
        }

        public Connection(PathSet pathSet, FixedParamSet paramSet) : base()
        {
            this.IsConnected = false;

            if (pathSet == null)
            {
                this.AddError("Constructor", "Required pathSet.");

                return;
            }

            if (paramSet == null)
            {
                this.AddError("Constructor", "Required paramSet.");

                return;
            }

            this._pathSet = pathSet.Clone();

            if (paramSet.SmbType == SmbType.Smb2)
            {
                this._client = this.GetConnection(SmbType.Smb2);
            }
            else if (paramSet.SmbType == SmbType.Smb1)
            {
                this._client = this.GetConnection(SmbType.Smb1);
            }
            else
            {
                this.AddError("Constructor", $"Unexpected ParamSet.SmbType: {paramSet.SmbType}");

                return;
            }

            if (this._client == null)
            {
                this.AddError("Constructor", "Connection Failed.");

                return;
            }

            if (!this.Login(paramSet))
            {
                this.AddError("Constructor", "Authentication Failed.");

                return;
            }

            this._paramSet = paramSet.Clone();

            this.IsConnected = true;
        }

        public Connection(Node node) : base()
        {
            this.IsConnected = false;

            if (node == null)
            {
                this.AddError("Constructor", "Required node.");

                return;
            }

            if (node.ParamSet == null || node.PathSet == null)
            {
                this.AddError("Constructor", "Invalid Node.");

                return;
            }

            this._pathSet = node.PathSet.Clone();

            if (node.ParamSet.SmbType == SmbType.Smb2)
            {
                this._client = this.GetConnection(SmbType.Smb2);
            }
            else if (node.ParamSet.SmbType == SmbType.Smb1)
            {
                this._client = this.GetConnection(SmbType.Smb1);
            }
            else
            {
                this.AddError("Constructor", $"Unexpected ParamSet.SmbType: {node.ParamSet.SmbType}");

                return;
            }

            if (this._client == null)
            {
                this.AddError("Constructor", "Connection Failed.");

                return;
            }

            if (!this.Login(node.ParamSet))
            {
                this.AddError("Constructor", "Authentication Failed.");

                return;
            }

            this._paramSet = node.ParamSet.Clone();

            this.IsConnected = true;
        }

        private ISMBClient GetConnection(SmbType smbType)
        {
            var result = (smbType == SmbType.Smb1)
                ? (ISMBClient)new SMB1Client()
                : (ISMBClient)new SMB2Client();

            return result.Connect(this._pathSet.IpAddress, SMBTransportType.DirectTCPTransport)
                ? result
                : null;
        }

        private bool Login(ParamSet paramSet)
        {
            var status = this._client.Login(
                paramSet.DomainName ?? string.Empty,
                paramSet.UserName ?? string.Empty,
                paramSet.Password ?? string.Empty
            );

            return (status == NTStatus.STATUS_SUCCESS);
        }

        private bool Login(FixedParamSet paramSet)
        {
            var status = this._client.Login(
                paramSet.DomainName ?? string.Empty,
                paramSet.UserName ?? string.Empty,
                paramSet.Password ?? string.Empty
            );

            return (status == NTStatus.STATUS_SUCCESS);
        }

        public Node GetNode()
        {
            if (!this.IsConnected)
                return null;

            if (string.IsNullOrEmpty(this._pathSet.Share))
            {
                // Requested Server
                return NodeFactory.Get(
                    this._pathSet.FullPath,
                    NodeType.Server,
                    this._paramSet
                );
            }

            // Requested Share, or Sub Node on Share
            using (var share = ShareFactory.Get(this._client, this._pathSet.Share))
            {
                if (!share.IsConnected)
                {
                    this.AddError("GetNode", $"Share Not Found: {this._pathSet.Share}");

                    return null;
                }

                Node result;
                if (this._pathSet.Elements.Length <= 0)
                {
                    // Requested Share
                    result = NodeFactory.Get(
                        this._pathSet.FullPath,
                        NodeType.Folder,
                        this._paramSet
                    );
                }
                else
                {
                    // Requested Sub Node on Share;
                    result = share.GetNode(this._pathSet, this._paramSet);
                    if (result == null)
                    {
                        this.CopyErrors(share);

                        return null;
                    }
                }

                return result;
            }
        }

        public Node[] GetList()
        {
            if (!this.IsConnected)
                return null;

            var names = this._client.ListShares(out var status);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                this.AddError("GetList", $"Share List Query Failed: {this._pathSet.Share}");

                return null;
            }

            return names
                .Select(e =>
                    NodeFactory.Get(
                        Utils.Combine(this._pathSet.IpAddressString, e),
                        NodeType.Folder,
                        this._paramSet
                    )
                )
                .ToArray();
        }

        public IShare GetShare()
        {
            return ShareFactory.Get(this._client, this._pathSet.Share);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        this._client?.Disconnect();
                    }
                    catch (Exception)
                    {
                    }

                    this._pathSet = null;
                    this._paramSet = null;
                    this._client = null;
                }

                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}

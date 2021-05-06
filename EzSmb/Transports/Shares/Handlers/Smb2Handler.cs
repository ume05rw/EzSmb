using EzSmb.Params.Enums;
using EzSmb.Transports.Shares.Handlers.Bases;
using EzSmb.Transports.Shares.Handlers.Enums;
using SMBLibrary.Client;

namespace EzSmb.Transports.Shares.Handlers
{
    /// <summary>
    /// Handle object manager for SMB2
    /// </summary>
    internal class Smb2Handler : HandlerBase
    {
        public Smb2Handler(
            ISMBFileStore store,
            string path,
            HandleType handleType,
            NodeType nodeType
        ) : base(SmbType.Smb2, store, path, handleType, nodeType)
        {
        }
    }
}

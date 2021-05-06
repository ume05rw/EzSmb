using EzSmb.Params.Enums;
using EzSmb.Transports.Shares.Handlers.Bases;
using EzSmb.Transports.Shares.Handlers.Enums;
using SMBLibrary.Client;

namespace EzSmb.Transports.Shares.Handlers
{
    /// <summary>
    /// Handle object manager for SMB1
    /// </summary>
    internal class Smb1Handler : HandlerBase
    {
        public Smb1Handler(
            ISMBFileStore store,
            string path,
            HandleType handleType,
            NodeType nodeType
        ) : base(SmbType.Smb1, store, path, handleType, nodeType)
        {
        }
    }
}

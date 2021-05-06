using SMBLibrary;

namespace EzSmb.Transports.Shares.Handlers
{
    internal class HandleArgs
    {
        internal AccessMask AccessMask { get; set; }
        internal FileAttributes FileAttributes { get; set; }
        internal ShareAccess ShareAccess { get; set; }
        internal CreateDisposition CreateDisposition { get; set; }
        internal CreateOptions CreateOptions { get; set; }
    }
}

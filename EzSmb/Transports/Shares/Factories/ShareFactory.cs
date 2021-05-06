using EzSmb.Transports.Shares.Interfaces;
using SMBLibrary.Client;

namespace EzSmb.Transports.Shares.Factories
{
    internal static class ShareFactory
    {
        public static IShare Get(ISMBClient client, string share)
        {
            if (client is SMB2Client)
                return new Smb2Share(client, share);
            if (client is SMB1Client)
                return new Smb1Share(client, share);

            // Returns Smb2Share object that holds the error.
            return new Smb2Share(null, share);
        }
    }
}

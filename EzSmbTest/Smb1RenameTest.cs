using EzSmbTest.Bases;
using SMBLibrary;
using SMBLibrary.Client;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Xunit;

namespace EzSmbTest
{
    public class Smb1RenameTest : TestBase
    {
        public Smb1RenameTest() : base()
        {
        }

        [Fact]
        public void RenameFile()
        {
            var setting = this.Settings.First();
            var client = new SMB1Client();
            var connected = client.Connect(IPAddress.Parse(setting.Address), SMBTransportType.DirectTCPTransport);
            if (connected)
            {
                var loginStatus = client.Login(string.Empty, setting.UserName, setting.Password);
                if (loginStatus == NTStatus.STATUS_SUCCESS)
                {
                    var store = client.TreeConnect(setting.TestPath.Share, out var shareStatus) as SMB1FileStore;
                    if (shareStatus == NTStatus.STATUS_SUCCESS)
                    {
                        var createStatus = store.CreateFile(
                            out var handle,
                            out _,
                            @"\\1.txt",
                            AccessMask.GENERIC_ALL
                                | AccessMask.SYNCHRONIZE,
                            0,
                            ShareAccess.None,
                            CreateDisposition.FILE_OPEN,
                            CreateOptions.FILE_NON_DIRECTORY_FILE,
                            null
                        );

                        if (createStatus == NTStatus.STATUS_SUCCESS)
                        {
                            var info = new FileRenameInformationType1()
                            {
                                FileName = @"\\2.txt"
                            };
                            var renameStatus = store.SetFileInformation(handle, info);

                            Debug.WriteLine((renameStatus == NTStatus.STATUS_SUCCESS)
                                ? "Succeeded!"
                                : "Failed...");

                            store.CloseFile(handle);
                        }

                        store.Disconnect();
                    }
                }

                client.Disconnect();
            }
        }

        [Fact]
        public void RenameFolder()
        {
            var setting = this.Settings.First();
            var client = new SMB1Client();
            var connected = client.Connect(IPAddress.Parse(setting.Address), SMBTransportType.DirectTCPTransport);
            if (connected)
            {
                var loginStatus = client.Login(string.Empty, setting.UserName, setting.Password);
                if (loginStatus == NTStatus.STATUS_SUCCESS)
                {
                    var store = client.TreeConnect(setting.TestPath.Share, out var shareStatus) as SMB1FileStore;
                    if (shareStatus == NTStatus.STATUS_SUCCESS)
                    {
                        var createStatus = store.CreateFile(
                            out var handle,
                            out _,
                            @"\\Musics\created",
                            AccessMask.GENERIC_ALL
                                | AccessMask.SYNCHRONIZE,
                            0,
                            ShareAccess.None,
                            CreateDisposition.FILE_OPEN,
                            CreateOptions.FILE_DIRECTORY_FILE,
                            null
                        );

                        if (createStatus == NTStatus.STATUS_SUCCESS)
                        {
                            var info = new FileRenameInformationType1()
                            {
                                FileName = @"\\updated",
                            };
                            var renameStatus = store.SetFileInformation(handle, info);

                            Debug.WriteLine((renameStatus == NTStatus.STATUS_SUCCESS)
                                ? "Succeeded!"
                                : "Failed...");

                            store.CloseFile(handle);
                        }

                        store.Disconnect();
                    }
                }

                client.Disconnect();
            }
        }
    }
}

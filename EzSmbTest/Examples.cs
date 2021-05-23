using EzSmb;
using EzSmb.Params;
using EzSmb.Params.Enums;
using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace EzSmbTest
{
    public static class Examples
    {
        public static async Task GetList()
        {
            // Get folder Node.
            var folder = await Node.GetNode(@"192.168.0.1\ShareName\FolderName", "userName", "password");

            // List items
            var nodes = await folder.GetList();
            foreach (var node in nodes)
            {
                Console.WriteLine($"Name: {node.Name}, Type: {node.Type}, LastAccessed: {node.LastAccessed:yyyy-MM-dd HH:mm:ss}");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:単純な 'using' ステートメントを使用する", Justification = "<保留中>")]
        public static async Task ReadFile()
        {
            // Get file Node.
            var file = await Node.GetNode(@"192.168.0.1\ShareName\FolderName\FileName.txt", "userName", "password");

            // Get MemoryStream.
            using (var stream = await file.Read())
            {
                var text = Encoding.UTF8.GetString(stream.ToArray());
                Console.WriteLine(text);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:メソッドの結果を無視しない", Justification = "<保留中>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:単純な 'using' ステートメントを使用する", Justification = "<保留中>")]
        public static async Task CreateFolderFile()
        {
            // Get Node of base folder.
            var folder = await Node.GetNode(@"192.168.0.1\ShareName\FolderName", "userName", "password");

            // Create Folder.
            var newFolder = await folder.CreateFolder("NewFolder");

            Console.WriteLine(newFolder.FullPath);
            // -> 192.168.0.1\ShareName\FolderName\NewFolder

            // Create Stream.
            var text = Encoding.UTF8.GetBytes("Hello!");
            using (var stream = new MemoryStream(text))
            {
                // Write Stream to child.
                await newFolder.Write(stream, "NewFile.txt");

                // Validate
                var newFile = await newFolder.GetNode("NewFile.txt");
                var writedStream = await newFile.Read();
                stream.ToArray().SequenceEqual(writedStream.ToArray());
            }
        }

        public static async Task Move()
        {
            // Get Node of file.
            var file = await Node.GetNode(@"192.168.0.1\ShareName\FileName.txt", "userName", "password");

            // Move to Child Path.
            var movedFile = await file.Move(@"FolderName\RenamedFileName.txt");

            Console.WriteLine(movedFile.FullPath);
            // -> 192.168.0.1\ShareName\FolderName\RenamedFileName.txt


            // Get Node of folder.
            var folder = await Node.GetNode(@"192.168.0.1\ShareName\FolderName\SubFolderName", "userName", "password");

            // Move to Parent Path.
            var movedFolder = await folder.Move(@"..\RenamedSubFolderName");

            Console.WriteLine(movedFolder.FullPath);
            // -> 192.168.0.1\ShareName\RenamedSubFolderName
        }

        public static async Task Delete()
        {
            // Get Node of file.
            var file = await Node.GetNode(@"192.168.0.1\ShareName\FileName.txt", "userName", "password");

            // Delete file.
            await file.Delete();


            // Get Node of folder.
            var folder = await Node.GetNode(@"192.168.0.1\ShareName\FolderName\SubFolderName", "userName", "password");

            // Delete file on folder.
            await folder.Delete("child.txt");

            // Delete folder.
            // ** If the folder still has subfolders or files, cannot delete it. **
            await folder.Delete();
        }

        public static async Task ScanServers()
        {
            // Get IP-Address String Array.
            var ipStrings = await Node.GetServers();

            foreach (var ipString in ipStrings)
            {
                try
                {
                    // Try Connect.
                    var server = await Node.GetNode(ipString);

                    // Connect Succeeded.
                    Console.WriteLine($"Server[{server.Name}] connected.");

                    // Get Share List.
                    var shares = await server.GetList();
                    foreach (var share in shares)
                    {
                        Console.WriteLine($"Found Share on Server[{server.Name}]: {share.Name}");
                    }
                }
                catch (Exception)
                {
                    // Connect Failed.
                    Console.WriteLine($"Server[{ipString}] requires authentication.");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:値の不必要な代入", Justification = "<保留中>")]
        public static async Task Authentication()
        {
            // Normally.
            var server1 = await Node.GetNode("192.168.0.1", "userName", "password");

            // Connect by Anonymous User.
            var server2 = await Node.GetNode("192.168.0.1");

            // Connect with specified SMB version.
            var server3 = await Node.GetNode("192.168.0.1", new ParamSet()
            {
                UserName = "userName",
                Password = "password",
                SmbType = SmbType.Smb1
            });

            // Connect by Windows-Domain Authentication. ** Not Supported SMB1 **
            var server4 = await Node.GetNode("192.168.0.1", new ParamSet()
            {
                UserName = "domainUserName",
                Password = "password",
                DomainName = "domainname.local"
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:単純な 'using' ステートメントを使用する", Justification = "<保留中>")]
        public static async Task RandomAccessByStream()
        {
            var file = await Node.GetNode(@"192.168.0.1\ShareName\FolderName\FileName.txt", "userName", "password");
            using (var stream = file.GetReaderStream())
            {
                var bytes1 = new byte[1024];
                var bytes2 = new byte[32];

                // Skip a few bytes.
                stream.Position = 64;
                var readed1 = stream.Read(bytes1, 0, 1024);
                Console.WriteLine(Encoding.UTF8.GetString(bytes1.Take(readed1).ToArray()));

                // Access from the end.
                stream.Seek(-32, SeekOrigin.End);
                var readed2 = await stream.ReadAsync(bytes2.AsMemory(0, 32));
                Console.WriteLine(Encoding.UTF8.GetString(bytes2.Take(readed2).ToArray()));
            }
        }

        public static async Task GetErrors()
        {
            try
            {
                // Set true the [throwException] arg, to throw a exception.
                var server1 = await Node.GetNode("192.168.0.1", "noSuchUser", "noSuchPassword", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }


            var file = await Node.GetNode(@"192.168.0.1\ShareName\FileName.txt", "userName", "password");
            await file.GetList();
            Console.WriteLine($"file has error? : {file.HasError}");
            // -> file has error? : true

            foreach (var err in file.Errors)
                Console.WriteLine(err);
            // -> [timestamp]: [namespace.class.method] error message.
        }
    }
}

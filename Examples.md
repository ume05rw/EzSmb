# Examples
* * *
#### Get items in shared folder:
```csharp
//using EzSmb;
//using System;

// Get folder Node.
var folder = await Node.GetNode(@"192.168.0.1\ShareName\FolderName", "userName", "password");

// List items
var nodes = await folder.GetList();
foreach (var node in nodes)
{
    Console.WriteLine($"Name: {node.Name}, Type: {node.Type}, LastAccessed: {node.LastAccessed:yyyy-MM-dd HH:mm:ss}");
}
```
* * *
#### Read file:
```csharp
//using EzSmb;
//using System;
//using System.Text;

// Get file Node.
var file = await Node.GetNode(@"192.168.0.1\ShareName\FolderName\FileName.txt", "userName", "password");

// Get MemoryStream.
using (var stream = await file.Read())
{
    var text = Encoding.UTF8.GetString(stream.ToArray());
    Console.WriteLine(text);
}
```
* * *
#### Create new folder/file:
```csharp
//using EzSmb;
//using System.IO;
//using System.Text;
//using System.Linq;

// Get Node of base folder.
var folder = await Node.GetNode(@"192.168.0.1\ShareName\FolderName", "userName", "password");

// Create new folder.
var newFolder = await folder.CreateFolder("NewFolder");

Console.WriteLine(newFolder.FullPath);
// -> 192.168.0.1\ShareName\FolderName\NewFolder


// Create stream for new file.
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
```
* * *
#### Move folder/file:

```csharp
// ** SMB1 NOT Supported. **

//using EzSmb;
//using System;

// Get Node of file.
var file = await Node.GetNode(@"192.168.0.1\ShareName\FileName.txt", "userName", "password");

// Move to child folder path.
// ** Even if you don't change the file/folder name, write the name. **
var movedFile = await file.Move(@"FolderName\RenamedFileName.txt");

Console.WriteLine(movedFile.FullPath);
// -> 192.168.0.1\ShareName\FolderName\RenamedFileName.txt


// Get Node of folder.
var folder = await Node.GetNode(@"192.168.0.1\ShareName\FolderName\SubFolderName", "userName", "password");

// Move to Parent path.
var movedFolder = await folder.Move(@"..\RenamedSubFolderName");

Console.WriteLine(movedFolder.FullPath);
// -> 192.168.0.1\ShareName\RenamedSubFolderName
```
* * *
#### Delete folder/file:

```csharp
//using EzSmb;
//using System;

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
```
* * *
#### Scan servers & shares on LAN:
```csharp
// ** Supported only IPv4 Private Address Network. **
// ** And, if the subnet range exceeds 1024, scan will skip that network interface. **

//using EzSmb;
//using System;

// Get IP-Address string array.
var ipStrings = await Node.GetServers();

foreach (var ipString in ipStrings)
{
    try
    {
        // Try connect.
        var server = await Node.GetNode(ipString);

        // Connect succeeded.
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
        // Connect failed.
        Console.WriteLine($"Server[{ipString}] requires authentication.");
    }
}
```
* * *
#### Authentication:

```csharp
//using EzSmb;
//using EzSmb.Params;
//using EzSmb.Params.Enums;

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
```
* * *
#### Get Errors:

```csharp
//using EzSmb;
//using System;

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
var nodes = await file.GetList();
// nodes is null.

Console.WriteLine($"file has error? : {file.HasError}");
// -> file has error? : true

foreach (var err in file.Errors)
    Console.WriteLine(err);
// -> [timestamp]: [namespace.class.method] error message.
```

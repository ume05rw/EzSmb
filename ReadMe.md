EzSmb
====

SMB(Windows shared folder) Clinet Library powered by [TalAloni's SmbLibrary](https://github.com/TalAloni/SMBLibrary), Xamarin & .NET Core Ready.  
  
## Description

It's easy to use, and supports SMB ver2 for Windows 10.  
Xamarin & .NET Core can access Windows Shared Folders and NAS without using mpr.dll or Netapi32.dll.  
Supports .Net Standard 2.0.  

## Requirement

[SMBLibrary](https://www.nuget.org/packages/SMBLibrary/) >= 1.4.6.1  
[NETStandard.Library](https://www.nuget.org/packages/NETStandard.Library/) >= 2.0.3

## Usage  

[Add NuGet Package](https://www.nuget.org/packages/EzSmb/) to your project.

```
> Install-Package EzSmb
```

and write code like this:

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

and more: [Examples](https://github.com/ume05rw/EzSmb/blob/master/Examples.md), [Namespace](https://github.com/ume05rw/EzSmb/blob/master/Namespace.md)


## Licence
[LGPL v3 Licence](https://github.com/ume05rw/EzSmb/blob/master/License.txt)

## Author
[Do-Be's](http://dobes.jp)

## Links

GitHub - TalAloni/SMBLibrary: SMB client & server implements.
[https://github.com/TalAloni/SMBLibrary](https://github.com/TalAloni/SMBLibrary)


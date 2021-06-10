EzSmb
====

SMB(Windows shared folder) Clinet Library powered by [TalAloni's SmbLibrary](https://github.com/TalAloni/SMBLibrary), Xamarin & .NET Core Ready.  

[![NuGet](https://img.shields.io/nuget/v/EzSmb.svg?label=NuGet)](https://www.nuget.org/packages/EzSmb/)

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
PM> Install-Package EzSmb
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

and more:  
- [Create new folder/file](https://github.com/ume05rw/EzSmb/blob/master/Examples.md#create-new-folderfile)
- [Move folder/file](https://github.com/ume05rw/EzSmb/blob/master/Examples.md#move-folderfile)
- [Delete folder/file](https://github.com/ume05rw/EzSmb/blob/master/Examples.md#delete-folderfile)
- [Scan servers & shares on LAN](https://github.com/ume05rw/EzSmb/blob/master/Examples.md#scan-servers--shares-on-lan)
- [Authentication](https://github.com/ume05rw/EzSmb/blob/master/Examples.md#authentication)
- [Random access by Stream](https://github.com/ume05rw/EzSmb/blob/master/Examples.md#random-access-by-stream)
- [Get errors](https://github.com/ume05rw/EzSmb/blob/master/Examples.md#get-errors)

Class Tree: [Namespace.md](https://github.com/ume05rw/EzSmb/blob/master/Namespace.md)


## Breaking changes:

The first argument of Node.GetList method has been changed since version 1.3.

```csharp
Node.GetList(string filter = "*", string relatedPath = null)
//           ^^ Inserted          ^^ move to second arg
```

## Licence
[LGPL v3 Licence](https://github.com/ume05rw/EzSmb/blob/master/License.txt)

## Author
[Do-Be's](http://dobes.jp)

## Contributors
[upcu](https://github.com/upcu)  
[synmra](https://github.com/synmra)


\#What did I do wrong operation? synmra was not added to sidebar...

## Links

GitHub - TalAloni/SMBLibrary: SMB client & server implements.  
[https://github.com/TalAloni/SMBLibrary](https://github.com/TalAloni/SMBLibrary)


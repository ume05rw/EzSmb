# Namespace

```
EzSmb
  |
  +-- Node
  |     |
  |     +-- static Task<Node> GetNode(string path, ParamSet paramSet = null, bool throwException = false)
  |     +-- static Task<Node> GetNode(string path, string userName, string password, bool throwException = false)
  |     +-- static Task<string[]> GetServers(int waitMsec = 0)
  |     |
  |     +-- string Name
  |     +-- string FullPath
  |     +-- NodeType Type
  |     +-- long? Size
  |     +-- DateTime? Created
  |     +-- DateTime? Updated
  |     +-- DateTime? LastAccessed
  |     +-- FixedParamSet ParamSet
  |     +-- PathSet PathSet
  |     |
  |     +-- Node GetParent()
  |     +-- Task<Node> GetNode(string relatedPath)
  |     +-- Task<Node[]> GetList(string filter = "*", string relatedPath = null)
  |     +-- ReaderStream GetReaderStream()
  |     +-- Task<ReaderStream> GetReaderStream(string relatedPath)
  |     +-- Task<MemoryStream> Read(string relatedPath = null)
  |     +-- Task<bool> Write(Stream stream, string relatedPath = null)
  |     +-- Task<Node> CreateFolder(string folderName, string relatedPath = null)
  |     +-- Task<bool> Delete(string relatedPath = null)
  |     +-- Task<Node> Move(string relatedNewPath, string relatedFromPath = null)
  |     +-- Dispose()
  |
  +-- Params
  |     |
  |     +-- ParamSet
  |     |     |
  |     |     +-- string UserName
  |     |     +-- string Password
  |     |     +-- string DomainName
  |     |     +-- SmbType? SmbType
  |     |
  |     +-- FixedParamSet
  |     |     +-- string UserName
  |     |     +-- string DomainName
  |     |     +-- SmbType SmbType
  |     |
  |     +-- Enums
  |           |
  |           +-- SmbType
  |
  +-- Paths
  |     |
  |     +-- PathSet
  |           |
  |           +-- string IpAddressString
  |           +-- IPAddress IpAddress
  |           +-- string Share
  |           +-- string[] Elements
  |           +-- string ElementsPath
  |           +-- string FullPath
  |
  +-- NodeType
  |
  +-- Streams
        |
        +-- ReaderStream
              |
              +-- bool CanRead
              +-- bool CanSeek
              +-- bool CanWrite
              +-- bool CanTimeout
              +-- long Length
              +-- int ReadTimeout
              +-- long Position
              +-- bool IsUseFileCache
              +-- bool HasError
              +-- string[] Errors
              |
              +-- void Flush()
              +-- long Seek(long offset, SeekOrigin origin)
              +-- int Read(byte[] buffer, int offset, int count)
              +-- void SetLength(long value)
              +-- void ClearErrors()
              +-- ... (various other methods of System.IO.Stream...)
```

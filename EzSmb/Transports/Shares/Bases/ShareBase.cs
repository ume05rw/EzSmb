using EzSmb.Params;
using EzSmb.Paths;
using EzSmb.Shareds;
using EzSmb.Shareds.Bases;
using EzSmb.Transports.Shares.Handlers.Enums;
using EzSmb.Transports.Shares.Handlers.Interfaces;
using EzSmb.Transports.Shares.Interfaces;
using SMBLibrary;
using SMBLibrary.Client;
using System;
using System.IO;
using System.Linq;

namespace EzSmb.Transports.Shares.Bases
{
    /// <summary>
    /// ISMBFileStore manager class
    /// </summary>
    internal abstract class ShareBase : ErrorManagedBase, IShare
    {
        private string _name;
        private ISMBFileStore _store;
        private bool disposedValue;

        protected abstract string PathPrefix { get; }
        protected ISMBFileStore Store => this._store;
        public bool IsConnected { get; }


        public ShareBase(ISMBClient client, string name)
        {
            this.IsConnected = false;
            this._name = name;

            if (client == null)
            {
                this.AddError("Constructor", "Required client.");

                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                this.AddError("Constructor", "Required name.");

                return;
            }

            this._store = client.TreeConnect(name, out var status);
            this.IsConnected = (status == NTStatus.STATUS_SUCCESS);
        }


        protected abstract IHandler GetHandler(string path, HandleType handleType, NodeType nodeType);
        public abstract Node[] GetList(Node node);
        protected abstract Node ExecMove(Node fromNode, Node toNode);

        protected virtual string FormatPath(string path)
        {
            return $@"{this.PathPrefix}{Utils.Resolve(path)}";
        }

        protected bool ValidateNode(Node node)
        {
            if (!this.IsConnected)
            {
                this.AddError("ValidateNode", "Not Connected.");

                return false;
            }

            if (node == null)
            {
                this.AddError("ValidateNode", "Required node.");

                return false;
            }

            return this.ValidatePathSet(node.PathSet);
        }

        protected bool ValidatePathSet(PathSet pathSet)
        {
            if (!this.IsConnected)
            {
                this.AddError("ValidatePathSet", "Not Connected.");

                return false;
            }

            if (pathSet == null)
            {
                this.AddError("ValidatePathSet", "Required pathSet.");

                return false;
            }

            if (string.IsNullOrEmpty(pathSet.Share))
            {
                this.AddError("ValidatePathSet", "Share Not Specified.");

                return false;
            }

            if (pathSet.Share.ToLower() != this._name.ToLower())
            {
                this.AddError("ValidatePathSet", $"Share Missmatch. this: {this._name}, args: {pathSet.Share}");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Get Node that PathSet specified.
        /// </summary>
        /// <param name="pathSet"></param>
        /// <param name="paramSet"></param>
        /// <returns></returns>
        public Node GetNode(PathSet pathSet, FixedParamSet paramSet)
        {
            if (!this.ValidatePathSet(pathSet))
                return null;

            if (pathSet.Elements.Length <= 0)
            {
                // Requested Share Folder.
                return NodeFactory.Get(
                    pathSet.FullPath,
                    NodeType.Folder,
                    paramSet
                );
            }
            else
            {
                // Requested Sub Node on Share.
                var result
                    = this.GetFolderNode(pathSet, paramSet)
                    ?? this.GetFileNode(pathSet, paramSet);

                if (result == null)
                {
                    this.AddError("GetNode", $"Path Not Found: {pathSet.FullPath}");

                    return null;
                }

                return result;
            }
        }

        private Node GetFolderNode(PathSet pathSet, FixedParamSet paramSet)
        {
            var path = this.FormatPath(pathSet.ElementsPath);
            using (var hdr = this.GetHandler(path, HandleType.Read, NodeType.Folder))
            {
                if (!hdr.Succeeded)
                    // DO NOT AddError on failed. Do only GetNode method.
                    return null;

                return this.CreateNode(hdr, NodeType.Folder, pathSet, paramSet);
            }
        }

        private Node GetFileNode(PathSet pathSet, FixedParamSet paramSet)
        {
            var path = this.FormatPath(pathSet.ElementsPath);
            using (var hdr = this.GetHandler(path, HandleType.Read, NodeType.File))
            {
                if (!hdr.Succeeded)
                    // DO NOT AddError on failed. Do only GetNode method.
                    return null;

                return this.CreateNode(hdr, NodeType.File, pathSet, paramSet);
            }
        }

        /// <summary>
        /// Query Infos, Create Node Instance.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="nodeType"></param>
        /// <param name="pathSet"></param>
        /// <param name="paramSet"></param>
        /// <returns></returns>
        protected Node CreateNode(
            IHandler handler,
            NodeType nodeType,
            PathSet pathSet,
            FixedParamSet paramSet
        )
        {
            if (handler == null)
            {
                this.AddError("CreateNode", "Required handler.");

                return null;
            }

            if (!handler.Succeeded)
            {
                this.AddError("CreateNode", "Invalid Handle.");

                return null;
            }

            if (nodeType != NodeType.File && nodeType != NodeType.Folder)
            {
                this.AddError("CreateNode", $"Invalid Operation: {nodeType}");

                return null;
            }

            var status = this.Store.GetFileInformation(
                out var basicInfo,
                handler.Handle,
                FileInformationClass.FileBasicInformation
            );

            if (status != NTStatus.STATUS_SUCCESS)
            {
                this.AddError("CreateNode", $"Basic Infomation Query Failed: {pathSet.FullPath}");

                return null;
            }

            FileInformation standardInfo = null;
            if (nodeType == NodeType.File)
            {
                status = this.Store.GetFileInformation(
                    out standardInfo,
                    handler.Handle,
                    FileInformationClass.FileStandardInformation
                );

                if (status != NTStatus.STATUS_SUCCESS)
                {
                    this.AddError("CreateNode", $"StandardInfomation Query Failed: {pathSet.FullPath}");

                    return null;
                }
            }

            return NodeFactory.Get(
                pathSet.FullPath,
                paramSet,
                (FileBasicInformation)basicInfo,
                (FileStandardInformation)standardInfo
            );
        }

        /// <summary>
        /// Get MemoryStream from File.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public MemoryStream Read(Node node)
        {
            if (!this.ValidateNode(node))
                return null;

            if (node.Type != NodeType.File)
            {
                this.AddError("Read", $"Invalid Operation: NodeType.{node.Type}");

                return null;
            }

            var path = this.FormatPath(node.PathSet.ElementsPath);
            using (var hdr = this.GetHandler(path, HandleType.Read, NodeType.File))
            {
                if (!hdr.Succeeded)
                {
                    this.AddError("Read", $"Create Handle Failed: {node.FullPath}");

                    return null;
                }

                return this.ReadStream(hdr);
            }
        }

        private MemoryStream ReadStream(IHandler handler)
        {
            var stream = new MemoryStream();
            long readed = 0;

            while (true)
            {
                var status = this.Store.ReadFile(
                    out var data,
                    handler.Handle,
                    readed,
                    (int)this.Store.MaxReadSize
                );

                if (
                    status != NTStatus.STATUS_SUCCESS
                    && status != NTStatus.STATUS_END_OF_FILE
                )
                {
                    stream.Dispose();
                    this.AddError("ReadStream", $"File Reading Failed.");

                    return null;
                }

                if (status == NTStatus.STATUS_END_OF_FILE || data.Length == 0)
                    break;

                readed += data.Length;
                stream.Write(data, 0, data.Length);
            }

            return stream;
        }

        /// <summary>
        /// Write Stream to File.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool Write(Node node, Stream stream)
        {
            if (!this.ValidateNode(node))
                return false;

            if (stream == null)
            {
                this.AddError("Write", "Required stream.");

                return false;
            }

            if (node.Type != NodeType.File)
            {
                this.AddError("Write", $"Invalid Operation: NodeType.{node.Type}");

                return false;
            }

            var path = this.FormatPath(node.PathSet.ElementsPath);
            using (var hdr = this.GetHandler(path, HandleType.Write, NodeType.File))
            {
                if (!hdr.Succeeded)
                {
                    this.AddError("Write", $"Create Handle Failed: {node.FullPath}");

                    return false;
                }

                return this.WriteStream(hdr, stream);
            }
        }

        private bool WriteStream(IHandler handler, Stream stream)
        {
            var writed = 0L;
            var buffer = new byte[this.Store.MaxWriteSize];
            while (true)
            {
                if (stream.Length <= writed)
                    break;

                try
                {
                    stream.Position = writed;
                }
                catch (Exception ex)
                {
                    this.AddError("WriteStream", $"Stream.Position Cannot Set.", ex);

                    return false;
                }

                var readed = stream.Read(buffer, 0, (int)this.Store.MaxWriteSize);
                var bytes = (readed == buffer.Length)
                    ? buffer
                    : buffer.Take(readed).ToArray();

                var status = this.Store.WriteFile(
                    out var numberOfBytesWritten,
                    handler.Handle,
                    writed,
                    bytes
                );

                if (status != NTStatus.STATUS_SUCCESS)
                {
                    this.AddError("WriteStream", $"File Writing Failed.");

                    return false;
                }

                writed += numberOfBytesWritten;
            }

            return true;
        }

        /// <summary>
        /// Create Sub Folder on Folder Node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public Node CreateFolder(Node node, string folderName)
        {
            if (!this.ValidateNode(node))
                return null;

            if (string.IsNullOrEmpty(folderName))
            {
                this.AddError("CreateFolder", "Required folderName");

                return null;
            }

            if (node.Type != NodeType.Folder)
            {
                this.AddError("CreateFolder", $"Invalid Operation: NodeType.{node.Type}");

                return null;
            }

            var newFullPath = $@"{node.FullPath}\{folderName}";
            PathSet newPathSet;
            try
            {
                newPathSet = PathSet.Parse(newFullPath);
            }
            catch (Exception ex)
            {
                this.AddError("CreateFolder", $"Invalid Path: {newFullPath}", ex);

                return null;
            }

            if (!this.ValidatePathSet(newPathSet))
                return null;

            var path = this.FormatPath(newPathSet.ElementsPath);
            using (var hdr = this.GetHandler(path, HandleType.Write, NodeType.Folder))
            {
                if (!hdr.Succeeded)
                {
                    this.AddError("CreateFolder", $"Create Handle Failed: {node.FullPath}");

                    return null;
                }

                // The creation handle cannot be used to get node information.
                //return this.CreateNode(hdr, NodeType.Folder, newPathSet, node.ParamSet);
            }

            using (var hdr = this.GetHandler(path, HandleType.Read, NodeType.Folder))
            {
                return this.CreateNode(hdr, NodeType.Folder, newPathSet, node.ParamSet);
            }
        }

        /// <summary>
        /// Delete Folder / File.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Delete(Node node)
        {
            if (!this.ValidateNode(node))
                return false;

            if (node.Type == NodeType.Server)
            {
                this.AddError("Delete", $"Invalid Operation: NodeType.{node.Type}");

                return false;
            }

            if (node.PathSet.Elements.Length <= 0)
            {
                this.AddError("Delete", $"Shared Folder Cannot Delete: {node.FullPath}");

                return false;
            }

            var path = this.FormatPath(node.PathSet.ElementsPath);
            using (var hdr = this.GetHandler(path, HandleType.Delete, node.Type))
            {
                if (!hdr.Succeeded)
                {
                    this.AddError("Delete", $"Create Handle Failed: {node.FullPath}");

                    return false;
                }

                var info = new FileDispositionInformation
                {
                    DeletePending = true
                };
                var status = this.Store.SetFileInformation(hdr.Handle, info);

                if (status != NTStatus.STATUS_SUCCESS)
                {
                    var message = (status == NTStatus.STATUS_DIRECTORY_NOT_EMPTY)
                        ? $"Directory is Not Empty: {node.FullPath}"
                        : $"Delete Failed: {node.FullPath}";

                    this.AddError("Delete", message);

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Move Folder / File.
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="relatedNewPath"></param>
        /// <returns></returns>
        public Node Move(Node fromNode, string relatedNewPath)
        {
            if (!this.ValidateNode(fromNode))
                return null;

            if (fromNode.Type == NodeType.Server)
            {
                this.AddError("Move", $"Invalid Operation: NodeType.{fromNode.Type}");

                return null;
            }

            if (fromNode.PathSet.Elements.Length <= 0)
            {
                this.AddError("Move", $"Shared Folder Cannot Move: {fromNode.PathSet.FullPath}");

                return null;
            }

            Node toNode;
            try
            {
                var toFullPath = Utils.Combine(
                    // move start point is parent node.
                    fromNode.GetParent().PathSet.FullPath,
                    relatedNewPath
                );
                toNode = NodeFactory.Get(toFullPath, fromNode.Type, fromNode.ParamSet);
            }
            catch (Exception ex)
            {
                this.AddError("Move", $"Path Resolution Failed: {relatedNewPath}", ex);

                return null;
            }

            if (
                fromNode.PathSet.IpAddressString != toNode.PathSet.IpAddressString
                || fromNode.PathSet.Share.ToLower() != toNode.PathSet.Share.ToLower()
            )
            {
                this.AddError("Move", $"Required Same Server & Same Shared Folder. from: {fromNode.FullPath}, to: {toNode.FullPath}");

                return null;
            }

            return this.ExecMove(fromNode, toNode);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        this._store?.Disconnect();
                    }
                    catch (Exception)
                    {
                    }

                    this._store = null;
                    this._name = null;
                }

                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}

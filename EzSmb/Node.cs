using EzSmb.Params;
using EzSmb.Paths;
using EzSmb.Scanners;
using EzSmb.Shareds;
using EzSmb.Shareds.Bases;
using EzSmb.Shareds.Interfaces;
using EzSmb.Streams;
using EzSmb.Transports;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EzSmb
{
    /// <summary>
    /// File, Folder, Server node class
    /// </summary>
    public class Node : ErrorManagedBase
    {
        #region "static"

        /// <summary>
        /// Get Node Instance.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="paramSet"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        /// <remarks>
        ///
        /// ** Warning **
        /// SMB1 with Windows Domain (= Active Directory) is NOT Supoorted.
        ///
        /// </remarks>
        public static async Task<Node> GetNode(
            string path,
            ParamSet paramSet = null,
            bool throwException = false
        )
        {
            PathSet pathSet;
            try
            {
                pathSet = PathSet.Parse(path);
            }
            catch (Exception ex)
            {
                var message = Node.GetErrorMessage(
                    "GetNode",
                    $"Invalid Format fullPath: {path}"
                );

                if (throwException)
                    throw new ArgumentException(message, ex);

                Debug.WriteLine(message);

                return null;
            }

            return await Task.Run(() =>
            {
                using (var conn = new Connection(pathSet, paramSet))
                {
                    if (conn.HasError)
                    {
                        if (throwException)
                            throw new IOException(Node.GetErrorMessage(conn));

                        Node.DebugWrite(conn);

                        return null;
                    }

                    var result = conn.GetNode();
                    if (result == null)
                    {
                        if (throwException)
                            throw new IOException(Node.GetErrorMessage(conn));

                        Node.DebugWrite(conn);

                        return null;
                    }

                    return result;
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Node Instance.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static Task<Node> GetNode(
            string path,
            string userName,
            string password,
            bool throwException = false
        )
        {
            var paramSet = new ParamSet()
            {
                UserName = userName ?? string.Empty,
                Password = password ?? string.Empty,
                DomainName = string.Empty
            };

            return Node.GetNode(path, paramSet, throwException);
        }

        /// <summary>
        /// Get Server's IP-Address string array on LAN.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Note:
        /// IPv6 Not Supported.
        ///
        /// Since the authentication information is unknown,
        /// it cannot be returned in the server node type.
        ///
        /// ** Warning **
        /// This implementation is a port scan to UDP port 137.
        /// It sends packets to all hosts on the subnet.
        /// Be aware that this may violate the security policy of the network.
        ///
        /// </remarks>
        public static async Task<string[]> GetServers(int waitMsec = 0)
        {
            using (var scanner = new Scanner())
            {
                var addresses = await scanner.Scan(waitMsec);

                return addresses
                    .Select(e => e.ToString())
                    .ToArray();
            }
        }

        private static void DebugWrite(IErrorManaged errorManaged)
        {
            if (errorManaged == null || !errorManaged.HasError)
                return;

            foreach (var error in errorManaged.Errors)
                Debug.WriteLine(error);
        }

        private static string GetErrorMessage(string methodName, string message)
            => $"{DateTime.Now:HH:mm:ss.fff}: [EzSmb.Node.{methodName}] {message}";
        private static string GetErrorMessage(IErrorManaged errorManaged)
            => (errorManaged == null || !errorManaged.HasError)
                ? string.Empty
                : string.Join(", ", errorManaged.Errors);


        #endregion "static"

        private Node _parent = null;
        private bool disposedValue;

        /// <summary>
        /// Node Name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Full Path string within Server, Folder
        /// </summary>
        public string FullPath => this.PathSet.FullPath;

        /// <summary>
        /// Node Type
        /// </summary>
        public NodeType Type { get; internal set; }

        /// <summary>
        /// File Size
        /// </summary>
        public long? Size { get; internal set; } = null;

        /// <summary>
        /// Created DateTime
        /// </summary>
        public DateTime? Created { get; internal set; } = null;

        /// <summary>
        /// Last Updated DateTime
        /// </summary>
        public DateTime? Updated { get; internal set; } = null;

        /// <summary>
        /// Last Access DateTime
        /// </summary>
        public DateTime? LastAccessed { get; internal set; } = null;

        /// <summary>
        /// Connection Parameters
        /// </summary>
        public FixedParamSet ParamSet { get; internal set; }

        /// <summary>
        /// Path Infomations
        /// </summary>
        public PathSet PathSet { get; set; }


        internal Node()
        {
        }

        /// <summary>
        /// Parent Node
        /// </summary>
        public Node GetParent()
        {
            if (this.Type == NodeType.Server)
                return null;

            if (this._parent != null)
                return this._parent;

            this._parent = NodeFactory.GetParent(this);

            return this._parent;
        }

        /// <summary>
        /// Get Node by Related Path
        /// </summary>
        /// <param name="relatedPath"></param>
        /// <returns></returns>
        public Task<Node> GetNode(string relatedPath)
            => this.GetNode(relatedPath, false);

        private async Task<Node> GetNode(string relatedPath, bool suppressErrors)
        {
            string fullPath;
            PathSet pathSet;

            try
            {
                fullPath = Utils.Combine(this.FullPath, relatedPath);
                pathSet = PathSet.Parse(fullPath);
            }
            catch (Exception ex)
            {
                if (!suppressErrors)
                    this.AddError("GetNode", ex.Message);

                return null;
            }

            return await Task.Run(() =>
            {
                using (var conn = new Connection(pathSet, this.ParamSet))
                {
                    if (conn.HasError)
                    {
                        if (!suppressErrors)
                            this.CopyErrors(conn);

                        return null;
                    }

                    var result = conn.GetNode();
                    if (result == null)
                    {
                        if (!suppressErrors)
                            this.CopyErrors(conn);

                        return null;
                    }

                    return result;
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Child Nodes.
        /// </summary>
        /// <param name="relatedPath"></param>
        /// <returns></returns>
        public async Task<Node[]> GetList(string filter="*", string relatedPath = null)
        {
            var node = await this.ResolveNode(relatedPath);
            if (node == null)
                return null;

            if (node != this)
            {
                var result = await node.GetList();
                if (result == null)
                {
                    this.CopyErrors(node);

                    return null;
                }

                return result;
            }

            if (this.Type == NodeType.File)
            {
                this.AddError("GetList", "Invalid Operation: NodeType.File");

                return null;
            }

            return await Task.Run(() =>
            {
                using (var conn = new Connection(this))
                {
                    if (conn.HasError)
                    {
                        this.CopyErrors(conn);

                        return null;
                    }

                    if (this.Type == NodeType.Server)
                    {
                        var shares = conn.GetList();
                        if (shares == null)
                        {
                            this.CopyErrors(conn);

                            return null;
                        }

                        return shares;
                    }
                    else if (this.Type == NodeType.Folder)
                    {
                        using (var share = conn.GetShare())
                        {
                            var result = share.GetList(this, filter);
                            if (result == null)
                            {
                                this.CopyErrors(share);

                                return null;
                            }

                            return result;
                        }
                    }

                    this.AddError("GetList", $"Unexpected NodeType: {this.Type}");

                    return null;
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Read Stream from File.
        /// </summary>
        /// <param name="relatedPath"></param>
        /// <returns></returns>
        public async Task<MemoryStream> Read(string relatedPath = null)
        {
            var node = await this.ResolveNode(relatedPath);
            if (node == null)
                return null;

            if (node != this)
            {
                var result = await node.Read();
                if (result == null)
                {
                    this.CopyErrors(node);

                    return null;
                }

                return result;
            }

            if (this.Type != NodeType.File)
            {
                this.AddError("Read", $"Invalid Operation: NodeType.{this.Type}");

                return null;
            }

            return await Task.Run(() =>
            {
                using (var conn = new Connection(this))
                using (var share = conn.GetShare())
                {
                    if (conn.HasError || share.HasError)
                    {
                        this.CopyErrors(conn);
                        this.CopyErrors(share);

                        return null;
                    }

                    var result = share.Read(this);
                    if (result == null)
                    {
                        this.CopyErrors(share);

                        return null;
                    }

                    return result;
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Write Stream to File.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="relatedPath"></param>
        /// <returns></returns>
        public async Task<bool> Write(Stream stream, string relatedPath = null)
        {
            var node = await this.ResolveNode(relatedPath, true);
            if (node == null)
            {
                // orderd new file?
                try
                {
                    var fullPath = Utils.Combine(this.FullPath, relatedPath);
                    node = NodeFactory.Get(fullPath, NodeType.File, this.ParamSet);
                }
                catch (Exception)
                {
                    // path invalid.
                    this.AddError("Write", $"Invalid relatedPath: {relatedPath}");

                    return false;
                }
            }

            if (node != this)
            {
                var result = await node.Write(stream);
                if (!result)
                {
                    this.CopyErrors(node);

                    return false;
                }

                return result;
            }

            if (this.Type != NodeType.File)
            {
                this.AddError("Write", $"Invalid Operation: NodeType.{this.Type}");

                return false;
            }

            return await Task.Run(() =>
            {
                using (var conn = new Connection(this))
                using (var share = conn.GetShare())
                {
                    if (conn.HasError || share.HasError)
                    {
                        this.CopyErrors(conn);
                        this.CopyErrors(share);

                        return false;
                    }

                    if (!share.Write(this, stream))
                    {
                        this.CopyErrors(share);

                        return false;
                    }

                    return true;
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Create Folder.
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="relatedPath"></param>
        /// <returns></returns>
        public async Task<Node> CreateFolder(string folderName, string relatedPath = null)
        {
            var node = await this.ResolveNode(relatedPath);
            if (node == null)
                return null;

            if (node != this)
            {
                var result = await node.CreateFolder(folderName);
                if (result == null)
                {
                    this.CopyErrors(node);

                    return null;
                }

                return result;
            }

            if (this.Type != NodeType.Folder)
            {
                this.AddError("CreateFolder", $"Invalid Operation: NodeType.{this.Type}");

                return null;
            }

            return await Task.Run(() =>
            {
                using (var conn = new Connection(this))
                using (var share = conn.GetShare())
                {
                    if (conn.HasError || share.HasError)
                    {
                        this.CopyErrors(conn);
                        this.CopyErrors(share);

                        return null;
                    }

                    var result = share.CreateFolder(this, folderName);
                    if (result == null)
                    {
                        this.CopyErrors(share);

                        return null;
                    }

                    return result;
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete Node.
        /// </summary>
        /// <param name="relatedPath"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string relatedPath = null)
        {
            var node = await this.ResolveNode(relatedPath);
            if (node == null)
                return false;

            if (node != this)
            {
                var result = await node.Delete();
                if (!result)
                {
                    this.CopyErrors(node);

                    return false;
                }

                return result;
            }

            if (this.Type == NodeType.Server)
            {
                this.AddError("Delete", $"Invalid Operation: NodeType.{this.Type}");

                return false;
            }

            if (this.PathSet.Elements.Length <= 0)
            {
                this.AddError("Delete", $"Shared Folder Cannot Delete: {this.PathSet.FullPath}");

                return false;
            }

            return await Task.Run(() =>
            {
                using (var conn = new Connection(this))
                using (var share = conn.GetShare())
                {
                    if (conn.HasError || share.HasError)
                    {
                        this.CopyErrors(conn);
                        this.CopyErrors(share);

                        return false;
                    }

                    if (!share.Delete(this))
                    {
                        this.CopyErrors(share);

                        return false;
                    }

                    return true;
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Move Node.
        /// </summary>
        /// <param name="relatedNewPath"></param>
        /// <param name="relatedFromPath"></param>
        /// <returns></returns>
        /// <remarks>
        ///
        /// ** SMB1 Not Supperted. **
        /// ** If you find a way, please let me know!! **
        /// This mehtod requires same server and same shared-folder.
        /// If it is different, will fail.
        ///
        /// </remarks>
        public async Task<Node> Move(string relatedNewPath, string relatedFromPath = null)
        {
            var node = await this.ResolveNode(relatedFromPath);
            if (node == null)
                return null;

            if (node != this)
            {
                var result = await node.Move(relatedNewPath);
                if (result == null)
                {
                    this.CopyErrors(node);

                    return null;
                }

                return result;
            }

            return await Task.Run(() =>
            {
                using (var conn = new Connection(this))
                using (var share = conn.GetShare())
                {
                    if (conn.HasError || share.HasError)
                    {
                        this.CopyErrors(conn);
                        this.CopyErrors(share);

                        return null;
                    }

                    var result = share.Move(this, relatedNewPath);
                    if (result == null)
                    {
                        this.CopyErrors(share);

                        return null;
                    }

                    return result;
                }
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Stream.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Stream is Read Only.
        /// </remarks>
        public ReaderStream GetReaderStream()
        {
            if (this.Type != NodeType.File)
            {
                this.AddError("GetReaderStream", $"Invalid Operation: NodeType.{this.Type}");

                return null;
            }

            return new ReaderStream(this);
        }


        /// <summary>
        /// Get Stream.
        /// </summary>
        /// <param name="relatedPath"></param>
        /// <returns></returns>
        /// <remarks>
        /// Stream is Read Only.
        /// </remarks>
        public async Task<ReaderStream> GetStream(string relatedPath)
        {
            var node = await this.ResolveNode(relatedPath);
            if (node == null)
                return null;

            if (node != this)
            {
                var result = node.GetReaderStream();
                if (result == null)
                {
                    this.CopyErrors(node);

                    return null;
                }

                return result;
            }

            return this.GetReaderStream();
        }

        private async Task<Node> ResolveNode(string relatedPath, bool suppressErrors = false)
        {
            if (
                string.IsNullOrEmpty(relatedPath)
                || Utils.Combine(this.FullPath, relatedPath) == this.FullPath
            )
            {
                // Requested current.
                return this;
            }
            else
            {
                // Requested another one.
                return await this.GetNode(relatedPath, suppressErrors);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this._parent?.Dispose();

                    this.Name = null;
                    this.Size = null;
                    this.Created = null;
                    this.Updated = null;
                    this.LastAccessed = null;
                    this.ParamSet = null;
                    this.PathSet = null;
                }

                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}

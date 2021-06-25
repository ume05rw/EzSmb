using EzSmb.Params;
using EzSmb.Paths;
using SMBLibrary;
using SMBLibrary.SMB1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EzSmb
{
    /// <summary>
    /// Get Node Instance from Path String.
    /// </summary>
    internal static class NodeFactory
    {
        private static Node InnerGet(
            NodeType nodeType,
            PathSet pathSet,
            FixedParamSet paramSet
        )
        {
            if (pathSet == null)
                throw new ArgumentException("Required pathSet.");
            if (paramSet == null)
                throw new ArgumentException("Required paramSet.");

            var result = new Node();
            switch (nodeType)
            {
                case NodeType.File:
                    {
                        if (pathSet.Elements.Length <= 0)
                            throw new ArgumentException($"Invalid File Path. : {pathSet.FullPath}");

                        result.Type = NodeType.File;
                        result.Name = pathSet.Elements.Last();

                        break;
                    }
                case NodeType.Folder:
                    {
                        if (string.IsNullOrEmpty(pathSet.Share))
                            throw new ArgumentException($"Invalid Folder Path. : {pathSet.FullPath}");

                        result.Type = NodeType.Folder;
                        result.Name = (0 < pathSet.Elements.Length)
                            ? pathSet.Elements.Last()
                            : pathSet.Share;

                        break;
                    }
                case NodeType.Server:
                    {
                        if (!string.IsNullOrEmpty(pathSet.Share))
                            throw new ArgumentException($"Invalid Server Path. : {pathSet.FullPath}");

                        result.Type = NodeType.Server;
                        result.Name = pathSet.IpAddressString;

                        break;
                    }
                default:
                    throw new Exception($"Unexpected NodeType: {nodeType}");
            }


            result.PathSet = pathSet;
            result.ParamSet = paramSet.Clone();

            return result;
        }

        /// <summary>
        /// Get Node from only Path.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="nodeType"></param>
        /// <param name="paramSet"></param>
        /// <returns></returns>
        public static Node Get(
            string fullPath,
            NodeType nodeType,
            FixedParamSet paramSet
        )
        {
            var pathSet = PathSet.Parse(fullPath);

            return NodeFactory.InnerGet(nodeType, pathSet, paramSet);
        }

        /// <summary>
        /// Get Node from Path and SMB-FileInfomation
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="paramSet"></param>
        /// <param name="basicInfo"></param>
        /// <param name="stdInfo"></param>
        /// <returns></returns>
        public static Node Get(
            string fullPath,
            FixedParamSet paramSet,
            FileBasicInformation basicInfo,
            FileStandardInformation stdInfo = null
        )
        {
            if (paramSet == null)
                throw new ArgumentException("Required paramSet.");
            if (basicInfo == null)
                throw new ArgumentException("Required info.");

            var pathSet = PathSet.Parse(fullPath);

            if (basicInfo.FileAttributes.HasFlag(SMBLibrary.FileAttributes.Directory))
            {
                // Folder
                var result = NodeFactory.InnerGet(
                    NodeType.Folder,
                    pathSet,
                    paramSet
                );
                result.Created = basicInfo.CreationTime;
                result.Updated = basicInfo.LastWriteTime;
                result.LastAccessed = basicInfo.LastAccessTime;

                return result;
            }
            else
            {
                // File
                var result = NodeFactory.InnerGet(
                    NodeType.File,
                    pathSet,
                    paramSet
                );
                result.Size = stdInfo?.EndOfFile;
                result.Created = basicInfo.CreationTime;
                result.Updated = basicInfo.LastWriteTime;
                result.LastAccessed = basicInfo.LastAccessTime;

                return result;
            }
        }

        /// <summary>
        /// Get Child Node from Path and Smb2-Result.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Node GetChild(
            Node parentNode,
            FileDirectoryInformation info
        )
        {
            if (parentNode == null)
                throw new ArgumentException("Required parentNode.");
            if (info == null)
                throw new ArgumentException("Required info.");

            var pathSet = PathSet.Parse($@"{parentNode.FullPath}\{info.FileName}");

            if (info.FileAttributes.HasFlag(SMBLibrary.FileAttributes.Directory))
            {
                // Folder
                var result = NodeFactory.InnerGet(
                    NodeType.Folder,
                    pathSet,
                    parentNode.ParamSet
                );
                result.Created = info.CreationTime;
                result.Updated = info.LastWriteTime;
                result.LastAccessed = info.LastAccessTime;

                return result;
            }
            else
            {
                // File
                var result = NodeFactory.InnerGet(
                    NodeType.File,
                    pathSet,
                    parentNode.ParamSet
                );
                result.Size = info.EndOfFile;
                result.Created = info.CreationTime;
                result.Updated = info.LastWriteTime;
                result.LastAccessed = info.LastAccessTime;

                return result;
            }
        }

        /// <summary>
        /// Get Child Node from Path and Smb1-Result.
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Node GetChild(
            Node parentNode,
            FindFileDirectoryInfo info
        )
        {
            if (parentNode == null)
                throw new ArgumentException("Required parentNode.");
            if (info == null)
                throw new ArgumentException("Required info.");

            var pathSet = PathSet.Parse($@"{parentNode.FullPath}\{info.FileName}");

            if (info.ExtFileAttributes.HasFlag(ExtendedFileAttributes.Directory))
            {
                // Folder
                var result = NodeFactory.InnerGet(
                    NodeType.Folder,
                    pathSet,
                    parentNode.ParamSet
                );
                result.Created = info.CreationTime;
                result.Updated = info.LastWriteTime;
                result.LastAccessed = info.LastAccessTime;

                return result;
            }
            else
            {
                // File
                var result = NodeFactory.InnerGet(
                    NodeType.File,
                    pathSet,
                    parentNode.ParamSet
                );
                result.Size = info.EndOfFile;
                result.Created = info.CreationTime;
                result.Updated = info.LastWriteTime;
                result.LastAccessed = info.LastAccessTime;

                return result;
            }
        }

        /// <summary>
        /// Get Parent Node from Node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Node GetParent(Node node)
        {
            if (node == null)
                throw new ArgumentException("Required node.");

            if (node.Type == NodeType.Server)
                return null;

            if (string.IsNullOrEmpty(node.PathSet.Share))
                throw new IOException($"Invalid FullPath String. : {node.FullPath}");

            var paths = new List<string>()
            {
                node.PathSet.IpAddressString
            };

            if (1 <= node.PathSet.Elements.Length)
                paths.Add(node.PathSet.Share);

            if (2 <= node.PathSet.Elements.Length)
                paths.AddRange(node.PathSet.Elements.Take(node.PathSet.Elements.Length - 1));

            var pathSet = PathSet.Parse(string.Join(@"\", paths));
            var nodeType = (string.IsNullOrEmpty(pathSet.Share))
                ? NodeType.Server
                : NodeType.Folder;

            return NodeFactory.InnerGet(nodeType, pathSet, node.ParamSet);
        }
    }
}

using EzSmb.Shareds;
using EzSmb.Transports.Shares.Bases;
using EzSmb.Transports.Shares.Handlers;
using EzSmb.Transports.Shares.Handlers.Enums;
using EzSmb.Transports.Shares.Handlers.Interfaces;
using SMBLibrary.Client;
using SMBLibrary.SMB1;
using System;
using System.Collections.Generic;

namespace EzSmb.Transports.Shares
{
    /// <summary>
    /// ISMBFileStore manager class for SMB1
    /// </summary>
    internal class Smb1Share : ShareBase
    {
        private static readonly string StaticPathPrefix = @"\\";
        protected override string PathPrefix => Smb1Share.StaticPathPrefix;
        private SMB1FileStore Smb1Store => (SMB1FileStore)this.Store;

        public Smb1Share(ISMBClient client, string share) : base(client, share)
        {
        }

        protected string FormatPath(string path, bool withSearch = false)
        {
            if (!withSearch)
                return base.FormatPath(path);

            var resolved = Utils.Resolve(path);

            return this.PathPrefix
                + resolved
                + (string.IsNullOrEmpty(resolved)
                    ? string.Empty
                    : @"\")
                + "*";
        }

        public override IHandler GetHandler(
            string path,
            HandleType handleType,
            NodeType nodeType
        )
        {
            return new Smb1Handler(this.Store, path, handleType, nodeType);
        }

        public override Node[] GetList(Node node)
        {
            if (!this.ValidateNode(node))
                return null;

            if (node.Type != NodeType.Folder)
            {
                this.AddError("GetList", $"Invalid Operation: NodeType.{node.Type}");

                return null;
            }

            var path = this.FormatPath(node.PathSet.ElementsPath);
            using (var hdr = this.GetHandler(path, HandleType.Read, NodeType.Folder))
            {
                if (!hdr.Succeeded)
                {
                    this.AddError("GetList", $"Create Handle Failed: {node.PathSet.FullPath}");

                    return null;
                }

                var searchPath = this.FormatPath(node.PathSet.ElementsPath, true);
                List<FindInformation> infos;
                try
                {
                    this.Smb1Store.QueryDirectory(
                        out infos,
                        searchPath,
                        FindInformationLevel.SMB_FIND_FILE_DIRECTORY_INFO
                    );
                }
                catch (Exception ex)
                {
                    this.AddError("GetList", $"List Query Failed: {node.PathSet.FullPath}", ex);

                    return null;
                }

                var list = new List<Node>();
                foreach (FindFileDirectoryInfo info in infos)
                {
                    if (
                        info.ExtFileAttributes == ExtendedFileAttributes.Directory
                        && (
                            info.FileName == "."
                            || info.FileName == ".."
                        )
                    )
                    {
                        continue;
                    }

                    list.Add(NodeFactory.GetChild(node, info));
                }

                return list.ToArray();
            }
        }

        protected override Node ExecMove(Node fromNode, Node toNode)
        {
            // On Windows XP SP3, I could not find a way.
            // In Samba 4.8.3, it was impossible to move the path,
            // only the node name could be changed.
            // The functionality is too limited,
            // so I gave up on implementation.

            this.AddError("ExecMove", $"Not Supported in SMB1.");

            return null;
        }
    }
}

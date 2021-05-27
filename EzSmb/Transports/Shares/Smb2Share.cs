using EzSmb.Transports.Shares.Bases;
using EzSmb.Transports.Shares.Handlers;
using EzSmb.Transports.Shares.Handlers.Enums;
using EzSmb.Transports.Shares.Handlers.Interfaces;
using SMBLibrary;
using SMBLibrary.Client;
using System;
using System.Collections.Generic;
using FileAttributes = SMBLibrary.FileAttributes;

namespace EzSmb.Transports.Shares
{
    /// <summary>
    /// ISMBFileStore manager class for SMB2
    /// </summary>
    internal class Smb2Share : ShareBase
    {
        private static readonly string StaticPathPrefix = string.Empty;
        protected override string PathPrefix => Smb2Share.StaticPathPrefix;
        private SMB2FileStore Smb2Store => (SMB2FileStore)this.Store;

        public Smb2Share(ISMBClient client, string share) : base(client, share)
        {
        }

        public override IHandler GetHandler(
            string path,
            HandleType handleType,
            NodeType nodeType
        )
        {
            return new Smb2Handler(this.Store, path, handleType, nodeType);
        }

        public override Node[] GetList(Node node, string filter = "*")
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

                List<QueryDirectoryFileInformation> infos;
                try
                {
                    this.Smb2Store.QueryDirectory(
                        out infos,
                        hdr.Handle,
                        filter,
                        FileInformationClass.FileDirectoryInformation
                    );
                }
                catch (Exception ex)
                {
                    this.AddError("GetList", $"List Query Failed: {node.PathSet.FullPath}", ex);

                    return null;
                }

                var list = new List<Node>();
                foreach (FileDirectoryInformation info in infos)
                {
                    if (
                        info.FileAttributes.HasFlag(FileAttributes.Directory)
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
            var fromPath = this.FormatPath(fromNode.PathSet.ElementsPath);

            // In SMB2, FileRenameInformationType2.FileName is the absolute path
            // on the shared folder.
            var toPath = this.FormatPath(toNode.PathSet.ElementsPath);

            using (var hdr = this.GetHandler(fromPath, HandleType.Move, fromNode.Type))
            {
                if (!hdr.Succeeded)
                {
                    this.AddError("ExecMove", $"Create Handle Failed: {fromNode.PathSet.FullPath}");

                    return null;
                }

                // SMB2
                var info = new FileRenameInformationType2
                {
                    FileName = toPath
                };
                var status = this.Store.SetFileInformation(hdr.Handle, info);

                if (status != NTStatus.STATUS_SUCCESS)
                {
                    if (status == NTStatus.STATUS_SHARING_VIOLATION)
                    {
                        this.AddError("ExecMove", $"Someone holds file / folder: {toNode.PathSet.FullPath}");
                    }
                    else
                    {
                        this.AddError("ExecMove", $"Move Failed: {status}, {toNode.PathSet.FullPath}");
                    }

                    return null;
                }
            }

            return toNode;
        }
    }
}

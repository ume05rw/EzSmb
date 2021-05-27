using EzSmb.Params;
using EzSmb.Paths;
using EzSmb.Shareds.Interfaces;
using EzSmb.Transports.Shares.Handlers.Enums;
using EzSmb.Transports.Shares.Handlers.Interfaces;
using SMBLibrary.Client;
using System;
using System.IO;

namespace EzSmb.Transports.Shares.Interfaces
{
    internal interface IShare : IErrorManaged, IDisposable
    {
        /// <summary>
        /// Connection Status
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// ISMBFileStore
        /// </summary>
        /// <remarks>
        /// MaxReadSize:
        ///   SMB1:
        ///   SMB2:
        /// </remarks>
        ISMBFileStore Store { get; }

        /// <summary>
        /// Format Path String
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string FormatPath(string path);

        /// <summary>
        /// Create Handle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="handleType"></param>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        IHandler GetHandler(string path, HandleType handleType, NodeType nodeType);

        /// <summary>
        /// Get Node
        /// </summary>
        /// <param name="pathSet"></param>
        /// <param name="paramSet"></param>
        /// <returns></returns>
        Node GetNode(PathSet pathSet, FixedParamSet paramSet);

        /// <summary>
        /// Get Child Node List
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        Node[] GetList(Node node, string filter = "*");

        /// <summary>
        /// Get MemoryStream of File
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        MemoryStream Read(Node node);

        /// <summary>
        /// Write File from Stream
        /// </summary>
        /// <param name="node"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        bool Write(Node node, Stream stream);

        /// <summary>
        /// Create Sub Folder
        /// </summary>
        /// <param name="node"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        Node CreateFolder(Node node, string folderName);

        /// <summary>
        /// Delete Node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool Delete(Node node);

        /// <summary>
        /// Move Node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="newPath"></param>
        /// <returns></returns>
        Node Move(Node node, string newPath);
    }
}

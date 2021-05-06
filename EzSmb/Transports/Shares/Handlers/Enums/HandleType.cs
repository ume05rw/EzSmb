using EzSmb.Params.Enums;
using SMBLibrary;
using System;

namespace EzSmb.Transports.Shares.Handlers.Enums
{
    internal enum HandleType
    {
        Read,
        Write,
        Delete,
        Move
    }

    internal static class HandleTypeExtension
    {
        public static HandleArgs ToArgs(
            this HandleType handleType,
            SmbType smbType,
            NodeType nodeType
        )
        {
            if (nodeType == NodeType.Server)
                throw new ArgumentException("NodeType.Server Not Supported.");

            var result = new HandleArgs()
            {
                FileAttributes = (handleType == HandleType.Move)
                    // !! INPORTANT !! Move(=Rename) to 0.
                    ? 0
                    // else, file or directory
                    : ((nodeType == NodeType.File)
                        ? FileAttributes.Normal
                        : FileAttributes.Directory),
                CreateOptions = (nodeType == NodeType.File)
                    ? CreateOptions.FILE_NON_DIRECTORY_FILE
                    : CreateOptions.FILE_DIRECTORY_FILE
            };

            if (smbType == SmbType.Smb2)
                result.CreateOptions |= CreateOptions.FILE_SYNCHRONOUS_IO_ALERT;

            switch (handleType)
            {
                case HandleType.Read:
                    result.AccessMask
                        = AccessMask.GENERIC_READ
                        | AccessMask.SYNCHRONIZE;
                    result.ShareAccess
                        = ShareAccess.Read
                        | ShareAccess.Write;
                    result.CreateDisposition = CreateDisposition.FILE_OPEN;

                    break;

                case HandleType.Write:
                    result.AccessMask
                        = AccessMask.GENERIC_WRITE
                        | AccessMask.SYNCHRONIZE;
                    result.ShareAccess = ShareAccess.None;

                    // * Create directory fail if exists.
                    result.CreateDisposition = (nodeType == NodeType.File)
                        ? CreateDisposition.FILE_SUPERSEDE
                        : CreateDisposition.FILE_CREATE;

                    break;

                case HandleType.Delete:
                    // AccessMask.GENERIC_ALL is failed in SMB1.
                    result.AccessMask
                        = AccessMask.GENERIC_WRITE
                        | AccessMask.DELETE
                        | AccessMask.SYNCHRONIZE;
                    result.ShareAccess = ShareAccess.None;
                    result.CreateDisposition = CreateDisposition.FILE_OPEN;

                    break;

                case HandleType.Move:
                    // AccessMask.GENERIC_WRITE is failed in SMB2. Reqired GENERIC_ALL.
                    result.AccessMask
                        = AccessMask.GENERIC_ALL
                        | AccessMask.SYNCHRONIZE;
                    result.ShareAccess = ShareAccess.Read;
                    result.CreateDisposition = CreateDisposition.FILE_OPEN;

                    break;

                default:
                    throw new ArgumentException($"Unexpected HandleType: {handleType}");
            }

            return result;
        }
    }
}

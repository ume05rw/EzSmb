using SMBLibrary;
using System;

namespace EzSmb.Transports.Shares.Handlers.Interfaces
{
    /// <summary>
    /// Handle manager interface
    /// </summary>
    public interface IHandler : IDisposable
    {
        /// <summary>
        /// Handle object
        /// </summary>
        object Handle { get; }

        /// <summary>
        /// FileStatus result
        /// </summary>
        FileStatus FileStatus { get; }

        /// <summary>
        /// Handle created flag
        /// </summary>
        bool Succeeded { get; }
    }
}

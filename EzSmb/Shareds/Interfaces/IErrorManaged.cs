using System;

namespace EzSmb.Shareds.Interfaces
{
    /// <summary>
    /// Interface with error string
    /// </summary>
    public interface IErrorManaged : IDisposable
    {
        /// <summary>
        /// Error string array
        /// </summary>
        string[] Errors { get; }

        /// <summary>
        /// Error flag
        /// </summary>
        bool HasError { get; }

        /// <summary>
        /// Clear error strings
        /// </summary>
        void ClearErrors();
    }
}

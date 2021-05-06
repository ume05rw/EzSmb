using System;

namespace EzSmb.Shareds.Interfaces
{
    public interface IErrorManaged : IDisposable
    {
        string[] Errors { get; }
        bool HasError { get; }
        void ClearErrors();
    }
}

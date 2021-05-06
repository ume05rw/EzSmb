using System;

namespace EzSmb.Transports.Shares.Handlers.Interfaces
{
    public interface IHandler : IDisposable
    {
        object Handle { get; }
        bool Succeeded { get; }
    }
}

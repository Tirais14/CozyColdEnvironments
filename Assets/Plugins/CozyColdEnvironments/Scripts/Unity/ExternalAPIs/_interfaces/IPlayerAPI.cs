using R3;
using System;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public interface IPlayerAPI : IDisposable
    {
        bool IsAuthorized { get; }

        void Authorize();

        Observable<bool> ObserveIsAuthorised();
    }
}

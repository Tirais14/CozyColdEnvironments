using CCEnvs.Unity.Profiles;
using R3;
using System;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public interface IPlayerAPI : IDisposable
    {
        bool IsAuthorized { get; }

        IUserProfile? PlayerPofile { get; }

        void Authorize();

        Observable<bool> ObserveIsAuthorised();
    }
}

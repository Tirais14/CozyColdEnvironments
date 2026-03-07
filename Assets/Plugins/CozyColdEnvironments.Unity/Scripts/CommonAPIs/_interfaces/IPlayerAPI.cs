using System;
using CCEnvs.Unity.Profiles;
using R3;

#nullable enable
namespace CCEnvs.Unity.CommonAPIs
{
    public interface IPlayerAPI : IDisposable
    {
        bool IsAuthorized { get; }

        IUserProfile? PlayerPofile { get; }

        void Authorize();

        Observable<bool> ObserveIsAuthorised();
    }
}

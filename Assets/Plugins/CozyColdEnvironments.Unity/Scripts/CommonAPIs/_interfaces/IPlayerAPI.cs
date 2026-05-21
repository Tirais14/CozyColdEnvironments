using System;
using CCEnvs.UnityX.Profiles;
using R3;

#nullable enable
namespace CCEnvs.UnityX.CommonAPIs
{
    public interface IPlayerAPI : IDisposable
    {
        bool IsAuthorized { get; }

        IUserProfile? PlayerPofile { get; }

        void Authorize();

        Observable<bool> ObserveIsAuthorised();
    }
}

using R3;

#nullable enable
namespace CCEnvs.Unity.ExternalAPIs
{
    public interface IPlayerAPI
    {
        bool IsAuthorized { get; }

        void Authorize();

        Observable<bool> ObserveIsAuthorised();
    }
}

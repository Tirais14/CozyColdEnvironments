using R3;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public interface IActivatable
    {
        bool IsActive { get; }

        Observable<bool> ObserveIsActive();
    }
}

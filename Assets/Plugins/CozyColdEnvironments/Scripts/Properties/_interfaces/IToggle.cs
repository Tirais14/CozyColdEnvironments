using R3;

#nullable enable
namespace CCEnvs.Proeprties
{
    public interface IToggle
    {
        bool State { get; set; }

        bool Trigger();

        Observable<bool> ObserveState();
    }
}

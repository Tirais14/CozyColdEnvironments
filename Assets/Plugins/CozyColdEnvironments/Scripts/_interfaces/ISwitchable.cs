#nullable enable

using R3;

namespace CCEnvs
{
    public interface ISwitchable
    {
        bool IsEnabled { get; }

        void Enable();

        void Disable();

        Observable<bool> ObserveEnabled();

        Observable<bool> ObserveDisabled();
    }
}
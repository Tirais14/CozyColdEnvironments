#nullable enable

namespace CCEnvs
{
    public interface ISwitchable
    {
        bool IsEnabled { get; }

        void Enable();

        void Disable();
    }
}
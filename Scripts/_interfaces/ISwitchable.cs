#nullable enable

namespace CozyColdEnvironments
{
    public interface ISwitchable
    {
        bool IsEnabled { get; }

        void Enable();

        void Disable();
    }
}
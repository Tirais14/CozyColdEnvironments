#nullable enable

namespace UTIRLib
{
    public interface IStateSwitchable
    {
        bool IsEnabled { get; }

        void Enable();

        void Disable();
    }
}
#nullable enable
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface IOpenable
    {
        IReadOnlyReactiveProperty<bool> IsOpened { get; }

        void Open();

        void Close();

        bool SwitchOpenableState();
    }
}
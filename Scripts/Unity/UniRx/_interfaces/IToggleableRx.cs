#nullable enable
using UniRx;

namespace CCEnvs.Unity.UniRx
{
    public interface IToggleableRx : IToggleable
    {
        new IReactiveProperty<bool> IsEnabled { get; }

        bool IToggleable.IsEnabled {
            get => IsEnabled.Value;
            set => IsEnabled.Value = value;
        }
    }
}

#nullable enable
using UniRx;

namespace CCEnvs.Unity.UniRx
{
    public interface IToggleableRx : IToggleable
    {
        global::UniRx.IReactiveProperty<bool> IsEnabledRx { get; }

        bool IToggleable.IsEnabled {
            get => IsEnabledRx.Value;
            set => IsEnabledRx.Value = value;
        }
    }
}

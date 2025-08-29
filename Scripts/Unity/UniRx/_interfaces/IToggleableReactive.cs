#nullable enable
using UniRx;

namespace CozyColdEnvironments.Reactive
{
    public interface IToggleableReactive : IToggleable
    {
        IReactiveProperty<bool> IsEnabledReactive { get; }

        bool IToggleable.IsEnabled {
            get => IsEnabledReactive.Value;
            set => IsEnabledReactive.Value = value;
        }
    }
}

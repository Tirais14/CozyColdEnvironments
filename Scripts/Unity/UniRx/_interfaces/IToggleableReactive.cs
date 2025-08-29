#nullable enable
using UniRx;

namespace UTIRLib.Reactive
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

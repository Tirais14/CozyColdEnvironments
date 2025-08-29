using UniRx;

#nullable enable
namespace CozyColdEnvironments.UI
{
    public interface ISelectableReactive : ISelectable
    {
        IReadOnlyReactiveProperty<bool> IsSelectedReactive { get; }
    }
}

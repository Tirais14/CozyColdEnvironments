using UniRx;

#nullable enable
namespace UTIRLib.UI
{
    public interface ISelectableReactive : ISelectable
    {
        IReadOnlyReactiveProperty<bool> IsSelectedProp { get; }
    }
}

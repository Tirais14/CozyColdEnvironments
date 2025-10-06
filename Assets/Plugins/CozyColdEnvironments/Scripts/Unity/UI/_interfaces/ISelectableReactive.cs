using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface ISelectableReactive : ISelectable
    {
        IReadOnlyReactiveProperty<bool> IsSelectedReactive { get; }
    }
}

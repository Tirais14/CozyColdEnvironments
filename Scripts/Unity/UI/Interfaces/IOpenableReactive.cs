using UniRx;

#nullable enable
namespace CozyColdEnvironments.UI
{
    public interface IOpenableReactive : IOpenable
    {
        IReadOnlyReactiveProperty<bool> IsOpenedReactive { get; }
    }
}

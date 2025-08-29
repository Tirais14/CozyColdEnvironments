using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IOpenableReactive : IOpenable
    {
        IReadOnlyReactiveProperty<bool> IsOpenedReactive { get; }
    }
}

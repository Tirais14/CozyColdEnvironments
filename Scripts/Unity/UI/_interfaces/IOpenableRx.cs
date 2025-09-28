using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IOpenableRx : IOpenable
    {
        IReadOnlyReactiveProperty<bool> IsOpenedRx { get; }
    }
}

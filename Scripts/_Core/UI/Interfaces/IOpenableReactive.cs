using UniRx;

#nullable enable
namespace UTIRLib.UI
{
    public interface IOpenableReactive : IOpenable
    {
        new IReadOnlyReactiveProperty<bool> IsOpenedReactive { get; }
    }
}

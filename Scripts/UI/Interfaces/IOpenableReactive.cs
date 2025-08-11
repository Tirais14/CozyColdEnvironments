using UniRx;

#nullable enable
namespace UTIRLib.UI
{
    public interface IOpenableReactive : IOpenable
    {
        IReadOnlyReactiveProperty<bool> IsOpenedReactive { get; }
    }
}

#nullable enable
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ITextViewModel : IViewModel
    {
        IReadOnlyReactiveProperty<string> TextView { get; }
    }
}

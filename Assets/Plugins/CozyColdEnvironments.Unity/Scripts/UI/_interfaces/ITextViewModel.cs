#nullable enable
using R3;

namespace CCEnvs.Unity.UI
{
    public interface ITextViewModel : IViewModel
    {
        ReadOnlyReactiveProperty<string> TextView { get; }
    }
}

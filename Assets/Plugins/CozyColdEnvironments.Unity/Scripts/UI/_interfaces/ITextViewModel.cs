#nullable enable
using R3;

namespace CCEnvs.UnityX.UI
{
    public interface ITextViewModel : IViewModel
    {
        ReadOnlyReactiveProperty<string> TextView { get; }
    }
}

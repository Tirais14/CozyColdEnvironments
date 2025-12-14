#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.UI;
using R3;

namespace CCEnvs.Unity.Items
{
    public interface IItemContainerViewModel : IViewModel, IIconViewModel
    {
        ReadOnlyReactiveProperty<string> CounterView { get; }
        Maybe<CompareAction<int>> ShowCounterTextPredicate { get; set; }
    }
}

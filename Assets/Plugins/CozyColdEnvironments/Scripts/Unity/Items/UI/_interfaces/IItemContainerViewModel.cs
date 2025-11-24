#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.UI;
using UniRx;

namespace CCEnvs.Unity.Items
{
    public interface IItemContainerViewModel : IViewModel, IIconViewModel
    {
        IReadOnlyReactiveProperty<string> CounterView { get; }
        Maybe<CompareAction<int>> ShowCounterTextPredicate { get; set; }
    }
}

#nullable enable
using CCEnvs.FuncLanguage;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ISelectionController<TKey, TValue>
    {
        IReadOnlyReactiveProperty<(TKey key, Maybe<TValue> it)> Selection { get; }

        void DoSelect(TKey key);

        void DoDeselect(TKey key);

        void SwitchSelectionState(TKey key);
    }
}

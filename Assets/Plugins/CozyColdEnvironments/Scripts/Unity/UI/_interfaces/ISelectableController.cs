#nullable enable
using CCEnvs.FuncLanguage;
using System.Collections.Generic;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ISelectableController<TKey, TValue>
    {
        IReadOnlyReactiveProperty<Maybe<KeyValuePair<TKey, TValue>>> Selection { get; }

        void DoSelect(TKey key);

        void DoDeselect(TKey key);

        void SwitchSelectionState(TKey key);
    }
}

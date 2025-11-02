#nullable enable
using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ISelectionController<TKey, TValue>
    {
        event Action<SelectionChangedEvent<TKey, TValue>> OnSelectionChanged;

        IReadOnlyReactiveProperty<KeyValuePair<TKey, Maybe<TValue>>> Selection { get; }

        IObservable<SelectionChangedEvent<TKey, TValue>> ObserveSelection();

        void DoSelect(TKey key);

        void DoDeselect(TKey key);

        void SwitchSelectionState(TKey key);
    }
}

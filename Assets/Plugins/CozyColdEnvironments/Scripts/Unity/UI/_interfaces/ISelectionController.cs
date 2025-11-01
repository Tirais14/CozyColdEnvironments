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

        void SelectItem(TKey key);

        void DeselectItem(TKey key);

        void SwitchSelectionState(TKey key);
    }
}

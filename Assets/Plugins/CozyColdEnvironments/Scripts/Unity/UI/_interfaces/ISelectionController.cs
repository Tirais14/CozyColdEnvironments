#nullable enable
using CCEnvs.Language;
using System;
using System.Collections.Generic;
using UniRx;

namespace CCEnvs.Unity.UI
{
    public interface ISelectionController<TKey, TValue>
    {
        event Action<SelectionChangedEvent<TKey, TValue>> OnSelectionChanged;

        IReadOnlyReactiveProperty<KeyValuePair<TKey, Ghost<TValue>>> Selection { get; }

        IObservable<SelectionChangedEvent<TKey, TValue>> ObserveSelection();

        void SelectIt(TKey key);

        void DeselectIt(TKey key);

        void SwitchSelectionState(TKey key);
    }
}

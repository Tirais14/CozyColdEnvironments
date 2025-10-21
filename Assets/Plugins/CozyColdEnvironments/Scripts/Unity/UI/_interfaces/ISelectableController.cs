#nullable enable
using CCEnvs.Language;
using System;

namespace CCEnvs.Unity.UI
{
    public interface ISelectableController<TKey, TValue>
    {
        event Action<Liquid<TValue>> OnSelectionChanged;

        Liquid<TValue> SelectionValue { get; }
        TKey SelectionKey { get; }

        IObservable<Liquid<TValue>> ObserveSelection();

        void Select(TKey key);

        void Deselect(TKey key);

        void SwitchSelectionState(TKey key);
    }
}

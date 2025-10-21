#nullable enable
using CCEnvs.Language;
using System;

namespace CCEnvs.Unity.UI
{
    public interface ISelectableController<TKey, TValue>
    {
        event Action<Ghost<TValue>> OnSelectionChanged;

        Ghost<TValue> SelectionValue { get; }
        TKey SelectionKey { get; }

        IObservable<Ghost<TValue>> ObserveSelection();

        void Select(TKey key);

        void Deselect(TKey key);

        void SwitchSelectionState(TKey key);
    }
}

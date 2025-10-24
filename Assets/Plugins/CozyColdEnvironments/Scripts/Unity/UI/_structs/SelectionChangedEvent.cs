#nullable enable
using CCEnvs.Language;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace CCEnvs.Unity.UI
{
    public static class SelectionChangedEvent
    {
        public static SelectionChangedEvent<TKey, TValue> Create<TKey, TValue>(
            TKey? previousKey,
            TValue? previousValue,
            TKey? newKey,
            TValue? newValue)
        {
            return new SelectionChangedEvent<TKey, TValue>(
                previousKey,
                previousValue,
                newKey,
                newValue);
        }
        public static SelectionChangedEvent<TKey, TValue> Create<TKey, TValue>(
            KeyValuePair<TKey, TValue> previous,
            TKey? newKey,
            TValue? newValue)
        {
            return new SelectionChangedEvent<TKey, TValue>(
                previous.Key,
                previous.Value,
                newKey,
                newValue);
        }
        public static SelectionChangedEvent<TKey, TValue> Create<TKey, TValue>(
            KeyValuePair<TKey, TValue> previous,
            KeyValuePair<TKey, TValue> @new)
        {
            return new SelectionChangedEvent<TKey, TValue>(
                previous.Key,
                previous.Value,
                @new.Key,
                @new.Value);
        }
    }
    public readonly struct SelectionChangedEvent<TKey, TValue>
    {
        public readonly Ghost<TKey> previousKey;
        public readonly Ghost<TValue> previousValue;
        public readonly Ghost<TKey> newKey;
        public readonly Ghost<TValue> newValue;

        public KeyValuePair<TKey, Ghost<TValue>> PreviousSelection => new(previousKey.Value()!, previousValue);
        public KeyValuePair<TKey, Ghost<TValue>> NewSelection => new(newKey.Value()!, newValue); 

        public SelectionChangedEvent(
            TKey? previousKey,
            TValue? previousValue,
            TKey? newKey,
            TValue? newValue)
        {
            this.previousKey = previousKey;
            this.previousValue = previousValue;
            this.newKey = newKey;
            this.newValue = newValue;
        }
    }
}

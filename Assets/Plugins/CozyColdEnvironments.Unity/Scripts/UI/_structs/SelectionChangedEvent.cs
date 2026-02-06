#nullable enable
using CCEnvs.FuncLanguage;
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
        public readonly Maybe<TKey> previousKey;
        public readonly Maybe<TValue> previousValue;
        public readonly Maybe<TKey> newKey;
        public readonly Maybe<TValue> newValue;

        public KeyValuePair<TKey, Maybe<TValue>> PreviousSelection => new(previousKey.GetValue()!, previousValue);
        public KeyValuePair<TKey, Maybe<TValue>> NewSelection => new(newKey.GetValue()!, newValue); 

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

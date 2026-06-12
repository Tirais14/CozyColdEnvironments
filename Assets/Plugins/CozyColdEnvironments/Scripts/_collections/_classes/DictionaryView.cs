using CCEnvs.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Collections
{
    public class DictionaryView<TKey, TValue, TValueView>
        :
        IReadOnlyDictionary<TKey, TValueView>
    {
        private readonly IReadOnlyDictionary<TKey, TValue> internalDictionary;

        private readonly Func<TValue, TValueView> valueConverter;

        public TValueView this[TKey key] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => valueConverter(internalDictionary[key]);
        }

        public IEnumerable<TKey> Keys {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => internalDictionary.Keys;
        }

        public IEnumerable<TValueView> Values {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => internalDictionary.Values.Select(valueConverter, (value, valueConverter) => valueConverter(value));
        }

        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => internalDictionary.Count;
        }

        public DictionaryView(IReadOnlyDictionary<TKey, TValue> dictionary, Func<TValue, TValueView> converter)
        {
            internalDictionary = dictionary;
            valueConverter = converter;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key) => internalDictionary.ContainsKey(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValueView value)
        {
            if (!internalDictionary.TryGetValue(key, out TValue untyped))
            {
                value = default!;
                return false;
            }

            value = valueConverter(untyped)!;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<KeyValuePair<TKey, TValueView>> GetEnumerator()
        {
            return internalDictionary.Select(valueConverter,
                (item, valueConverter) =>
                {
                    return KeyValuePair.Create(item.Key, valueConverter(item.Value));
                })
                .GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

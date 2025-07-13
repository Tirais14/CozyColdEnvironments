using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.DB
{
    public interface IDatabase
    {
        int Count { get; }
        bool IsNotEmpty { get; }

        object GetValue(object key);

        void AddValue(IValuePair value);

        void AddValues(params IValuePair[] values);

        bool ContainsKey(object? key);

        bool Contains(object? value);
    }

    public interface IDatabase<TKey, TValue> : IDatabase, IEnumerable<TValue>
    {
        TValue this[TKey key] { get; }

        void AddValue(IValuePair<TKey, TValue> value);

        void AddValues(params IValuePair<TKey, TValue>[] values);

        bool ContainsKey(TKey? key);

        bool Contains(TValue? value);

        TValue? FindValue(Type type);

        TValue? FindValue(Predicate<TValue> objectFilter);

        TValue? FindValue(Predicate<TKey> keyFilter, Predicate<TValue> objectFilter);

        TValue? FindValueByKey(Predicate<TKey> keyFilter);

        T? FindValueT<T>();

        bool TryFindValue(Type type, [NotNullWhen(true)] out TValue? value);

        bool TryFindValue(Predicate<TValue> objectFilter, [NotNullWhen(true)] out TValue? value);

        bool TryFindValue(Predicate<TKey> keyFilter, Predicate<TValue> objectFilter,
            [NotNullWhen(true)] out TValue? value);

        bool TryFindValueByKey(Predicate<TKey> keyFilter, [NotNullWhen(true)] out TValue? value);

        bool TryFindValueT<T>([NotNullWhen(true)] out T? value);

        TValue[] FindValues(Type type);

        TValue[] FindValues(Predicate<TValue> objectFilter);

        TValue[] FindValues(Predicate<TKey> keyFilter, Predicate<TValue> objectFilter);

        TValue[] FindValuesByKey(Predicate<TKey> keyFilter);

        T[] FindValuesT<T>();

        bool TryFindValues(Type type, out TValue[] values);

        bool TryFindValues(Predicate<TValue> objectFilter, out TValue[] values);

        bool TryFindValues(Predicate<TKey> keyFilter, Predicate<TValue> objectFilter, out TValue[] values);

        bool TryFindValuesByKey(Predicate<TKey> keyFilter, out TValue[] values);

        bool TryFindValuesT<T>(out T[] values);

        TValue GetValue(TKey key);

        TValue? GetValue(Type type, TKey key);

        T? GetValueT<T>(TKey key);

        bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value);

        bool TryGetValue(Type type, TKey key, [NotNullWhen(true)] out TValue? value);

        bool TryGetValueT<T>(TKey key, [NotNullWhen(true)] out T? value);
    }
}
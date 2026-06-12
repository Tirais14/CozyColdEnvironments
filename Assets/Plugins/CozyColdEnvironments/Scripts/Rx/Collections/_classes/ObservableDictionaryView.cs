//using CCEnvs.Linq;
//using ObservableCollections;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using UnityEngine;

//#nullable enable
//namespace CCEnvs.Rx.Collections
//{
//    public class ObservableDictionaryView<TKey, TValue, TValueView>
//        :
//        IReadOnlyObservableDictionary<TKey, TValueView>
//    {
//        private readonly IReadOnlyObservableDictionary<TKey, TValue> internalDictionary;

//        private readonly Func<TValue, TValueView> valueConverter;

//        public TValueView this[TKey key] {
//            [MethodImpl(MethodImplOptions.AggressiveInlining)]
//            get => valueConverter(internalDictionary[key]);
//        }

//        public IEnumerable<TKey> Keys {
//            [MethodImpl(MethodImplOptions.AggressiveInlining)]
//            get => internalDictionary.Keys;
//        }

//        public IEnumerable<TValueView> Values {
//            [MethodImpl(MethodImplOptions.AggressiveInlining)]
//            get => internalDictionary.Values.Select(valueConverter, (value, valueConverter) => valueConverter(value));
//        }

//        public object SyncRoot {
//            [MethodImpl(MethodImplOptions.AggressiveInlining)]
//            get => internalDictionary.SyncRoot;
//        }

//        public int Count {
//            [MethodImpl(MethodImplOptions.AggressiveInlining)]
//            get => internalDictionary.Count;
//        }

//        public event NotifyCollectionChangedEventHandler<KeyValuePair<TKey, TValueView>>? CollectionChanged {
//            add => internalDictionary.CollectionChanged += (ev) => val;
//            remove => internalDictionary.CollectionChanged += value
//        }

//        public bool ContainsKey(TKey key)
//        {
//            throw new NotImplementedException();
//        }

//        public ISynchronizedView<KeyValuePair<TKey, TValueView>, TView> CreateView<TView>(Func<KeyValuePair<TKey, TValueView>, TView> transform)
//        {
//            throw new NotImplementedException();
//        }

//        public bool TryGetValue(TKey key, out TValueView value)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerator<KeyValuePair<TKey, TValueView>> GetEnumerator()
//        {
//            internalDictionary.SelectValue()
//        }
//        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//    }
//}

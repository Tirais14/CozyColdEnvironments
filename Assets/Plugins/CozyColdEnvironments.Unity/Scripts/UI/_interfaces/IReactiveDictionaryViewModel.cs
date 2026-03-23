using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;

#nullable enable
namespace CCEnvs.Unity.UI
{
    [Obsolete]
    public interface IReactiveDictionaryViewModel<TKey, TValue> : IViewModel
    {
        void Add(TKey key, TValue value);

        void Remove(TKey key);

        void Remove(TKey key, TValue value);

        Observable<DictionaryAddEvent<TKey, TValue>> ObserveAdd();

        Observable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove();

        Observable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace();

        Observable<CollectionResetEvent<KeyValuePair<TKey, TValue>>> ObserveReset();
    }
}

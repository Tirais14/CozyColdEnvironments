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
        ReactiveCommand<KeyValuePair<TKey, TValue>> Add { get; }

        ReactiveCommand<TKey> Remove { get; }

        ReactiveCommand<KeyValuePair<TKey, TValue>> Replace { get; }

        Observable<DictionaryAddEvent<TKey, TValue>> ObserveAdd();

        Observable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove();

        Observable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace();

        Observable<CollectionResetEvent<KeyValuePair<TKey, TValue>>> ObserveReset();
    }
}

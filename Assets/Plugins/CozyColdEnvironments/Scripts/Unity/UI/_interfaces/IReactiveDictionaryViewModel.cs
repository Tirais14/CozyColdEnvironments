using ObservableCollections;
using R3;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IReactiveDictionaryViewModel<TKey, TValue> : IViewModel
    {
        ReactiveCommand<KeyValuePair<TKey, TValue>> Add { get; }

        ReactiveCommand<TKey> Remove { get; }

        ReactiveCommand<KeyValuePair<TKey, TValue>> Replace { get; }

        Observable<DictionaryAddEvent<TKey, TValue>> ObserveAddContainer();

        Observable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemoveContainer();

        Observable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplaceContainer();

        Observable<CollectionResetEvent<KeyValuePair<TKey, TValue>>> ObserveResetContainer();
    }
}

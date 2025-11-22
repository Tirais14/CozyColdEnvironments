using System;
using System.Collections.Generic;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IReactiveDictionaryViewModel<TKey, TValue> : IViewModel
    {
        IReactiveCommand<KeyValuePair<TKey, TValue>> Add { get; }

        IReactiveCommand<TKey> Remove { get; }

        IReactiveCommand<KeyValuePair<TKey, TValue>> Replace { get; }

        IObservable<DictionaryAddEvent<TKey, TValue>> ObserveAdd();

        IObservable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove();

        IObservable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace();
    }
}

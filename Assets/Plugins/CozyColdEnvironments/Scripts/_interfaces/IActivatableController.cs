#nullable enable
using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;

namespace CCEnvs.Unity
{
    public interface IActivatableController<TKey, TValue>
    {
        Maybe<KeyValuePair<TKey, TValue>> ActiveObject { get; }

        void ActivateAt(TKey key);

        void DeactivateAt(TKey key);

        bool SwitchActiveStateAt(TKey key);

        IObservable<PreviousCurrentPair<Maybe<KeyValuePair<TKey, TValue>>>> ObserveActiveNode();
    }
}

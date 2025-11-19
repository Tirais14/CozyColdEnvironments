#nullable enable
using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;

namespace CCEnvs.Unity
{
    public interface IActivatableController<TKey, TNode>
    {
        Maybe<KeyValuePair<TKey, TNode>> ActiveNode { get; }

        void ActivateAt(TKey key);

        void DeactivateAt(TKey key);

        bool SwitchActiveStateAt(TKey key);

        IObservable<PreviousCurrentPair<Maybe<KeyValuePair<TKey, TNode>>>> ObserveActiveNode();
    }
}

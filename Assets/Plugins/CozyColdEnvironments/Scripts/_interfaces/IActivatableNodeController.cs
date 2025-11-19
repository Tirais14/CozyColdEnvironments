#nullable enable
using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;

namespace CCEnvs.Unity
{
    public interface IActivatableNodeController<TKey, TNode>
    {
        Maybe<KeyValuePair<TKey, TNode>> ActiveNode { get; }

        void ActivateNode(TKey key);

        void DeactivateNode(TKey key);

        bool SwitchActiveState(TKey key);

        IObservable<PreviousCurrentPair<Maybe<KeyValuePair<TKey, TNode>>>> ObserveActiveNode(Maybe<TKey> key);

        IObservable<PreviousCurrentPair<KeyValuePair<TKey, TNode>>> ObserveActivateNode(Maybe<TKey> key);

        IObservable<PreviousCurrentPair<KeyValuePair<TKey, TNode>>> ObserveDeactivateNode(Maybe<TKey> key);

        IObservable<PreviousCurrentPair<KeyValuePair<TKey, TNode>>> ObserveNodeActiveState(Maybe<TKey> key);
    }
}

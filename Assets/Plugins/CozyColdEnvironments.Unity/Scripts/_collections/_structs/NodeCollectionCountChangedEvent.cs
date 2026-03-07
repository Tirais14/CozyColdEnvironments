using System.Collections.Generic;
using CCEnvs.FuncLanguage;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public readonly struct NodeCollectionCountChangedEvent<TKey, TNode>
    {
        public KeyValuePair<TKey, TNode> Node { get; init; }
        public Maybe<GameObject> NodeGameObject { get; init; }
        public bool GameObjectInstantiated { get; init; }
    }
}

using CCEnvs.FuncLanguage;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public readonly struct NodeCollectionInstantiateEvent<TNode>
    {
        public Maybe<TNode> InputNode { get; init; }
        public TNode GameObjectNode { get; init; }
        public GameObject Instantiated { get; init; }
    }
}

using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public class ActivatableNodeCollection<TKey, TNode>
        : NodeCollection<TKey, TNode>,
        IActivatableNodeController<TKey, TNode>
        where TNode : IActivatableNode
    {
        
    }
}

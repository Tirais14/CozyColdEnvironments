using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    //public class ActivatableNodeCollection<TKey, TNode>
    //    : NodeCollection<TKey, TNode>,
    //    IActivatableController<TKey, TNode>
    //    where TNode : IActivatable
    //{
    //    protected readonly ReactiveProperty<Maybe<KeyValuePair<TKey, TNode>>> activeNode = new();
    //    protected readonly List<IDisposable> disposables = new();

    //    public Maybe<KeyValuePair<TKey, TNode>> ActiveObject => activeNode.Value;

    //    public ActivatableNodeCollection()
    //    {
    //    }

    //    public ActivatableNodeCollection(int capacity)
    //        :
    //        base(capacity)
    //    {
    //    }

    //    public ActivatableNodeCollection(IEnumerable<KeyValuePair<TKey, TNode>> nodes)
    //        :
    //        base(nodes)
    //    {
    //    }

    //    public ActivatableNodeCollection(int nodeCount, GameObject nodePrefab)
    //        :
    //        base(nodeCount, nodePrefab)
    //    {
    //    }

    //    public void ActivateAt(TKey key)
    //    {
    //        collection[key].Activate();
    //    }

    //    public void DeactivateAt(TKey key)
    //    {
    //        collection[key].Deactivate();
    //    }

    //    public bool SwitchActiveStateAt(TKey key)
    //    {
    //        return collection[key].SwitchActiveState();
    //    }

    //    public IObservable<PreviousCurrentPair<Maybe<KeyValuePair<TKey, TNode>>>> ObserveActiveNode()
    //    {
    //        return activeNode.Pairwise().Select(pair => PreviousCurrentPair.Create(pair.Previous, pair.Current));
    //    }

    //    protected override void OnAdd(TKey key, TNode node)
    //    {
    //        base.OnAdd(key, node);

    //        node.ObserveActivate()
    //            .SubscribeWithState2(activeNode, KeyValuePair.Create(key, node),
    //                static (_, prop, node) =>
    //                {
    //                    if (prop.Value == node)
    //                        return;

    //                    prop.Value.IfSome(n => n.Value.Deactivate());
    //                    prop.Value = node;
    //                })
    //            .AddTo(disposables);
    //    }

    //    protected override void OnRemove(TKey key, TNode node)
    //    {
    //        base.OnRemove(key, node);

    //        node.ObserveDeactivate()
    //            .SubscribeWithState(activeNode,
    //                static (_, prop) =>
    //                {
    //                    prop.Value.IfSome(n => n.Value.Deactivate());
    //                    prop.Value = Maybe<KeyValuePair<TKey, TNode>>.None;
    //                })
    //            .AddTo(disposables);
    //    }

    //    private bool disposed;
    //    protected override void Dispose(bool disposing)
    //    {
    //        base.Dispose(disposing);

    //        if (disposed)
    //            return;

    //        if (disposing)
    //            disposables.DisposeAll();

    //        disposed = true;
    //    }
    //}
}

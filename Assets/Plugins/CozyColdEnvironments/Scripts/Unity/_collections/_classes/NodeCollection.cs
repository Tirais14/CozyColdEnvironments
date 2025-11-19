using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public class NodeCollection<TKey, TNode> : INodeCollection<TKey, TNode>
    {
        private readonly Dictionary<TKey, TNode> collection = new();
        private readonly ReactiveProperty<Maybe<KeyValuePair<TKey, TNode>>> activeNode = new();
        private Subject<Unit>? clearSubj;
        private Subject<KeyValuePair<TKey, TNode>>? addNodeSubj;
        private Subject<KeyValuePair<TKey, TNode>>? removeNodeSubj;
        private Subject<PreviousCurrentPair<KeyValuePair<TKey, TNode>>>? replaceNodeSubj;

        public TNode this[TKey key] {
            get => collection[key];
            set
            {
                var t = collection[key];
                collection[key] = value;

                KeyValuePair<TKey, TNode> previous = KeyValuePair.Create(key, t);
                KeyValuePair<TKey, TNode> current = KeyValuePair.Create(key, value);

                replaceNodeSubj?.OnNext(PreviousCurrentPair.Create(previous, current));
            }
        }

        public IEnumerable<TKey> Keys => collection.Keys;
        public IEnumerable<TNode> Nodes => collection.Values;
        public Maybe<KeyValuePair<TKey, TNode>> ActiveNode {
            get => activeNode.Value;
            set => activeNode.Value = value;
        }
        public Maybe<Func<IEnumerable<KeyValuePair<TKey, TNode>>, TNode, TKey>> KeyFactory { get; set; }
        public Func<TNode, GameObject, TNode> NodePrefabInstantiatedProcessor { get; private set; }
        public Maybe<GameObject> NodePrefab { get; set; }
        public Maybe<GameObject> gameObject { get; private set; }

        public NodeCollection(Func<TNode, GameObject, TNode> nodePrefabInstantiatedProcessor)
        {
            this.NodePrefabInstantiatedProcessor = nodePrefabInstantiatedProcessor;
        }

        public void SetNodePrefabInstantiatedProcessor(
            Func<TNode, GameObject, TNode> func)
        {
            Guard.IsNotNull(func);

            NodePrefabInstantiatedProcessor = func;
        }

        public bool AddNode(TKey key, TNode node)
        {
            if (collection.ContainsKey(key))
                return false;

            collection.Add(key, node);
            addNodeSubj?.OnNext(KeyValuePair.Create(key, node));

            return true;
        }

        public bool AddNode(TNode node)
        {
            if (KeyFactory.IsNone)
                CC.Throw.InvalidOperation(KeyFactory, nameof(KeyFactory));

            TKey key = KeyFactory.GetValueUnsafe().Invoke(this, node);
            return AddNode(key, node); 
        }

        public TNode AddNodeByPrefab(TKey key, TNode node, GameObject prefab)
        {
            CC.Guard.IsNotNull(prefab, nameof(prefab));

            TNode newNode = NodePrefabInstantiatedProcessor(node, Object.Instantiate(prefab));
            AddNode(key, newNode);

            return newNode;
        }

        public TNode AddNodeByPrefab(TKey key, TNode node)
        {
            if (NodePrefab.IsNone)
                CC.Throw.InvalidOperation(NodePrefab, nameof(NodePrefab));

            return AddNodeByPrefab(key, node, NodePrefab.GetValueUnsafe());
        }

        public bool BindGameObject(GameObject gameObject)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            return this.gameObject.BiMap(
                some: go => false,
                none: () =>
                {
                    this.gameObject = gameObject;

                    return true;
                }).Raw;
        }

        public Maybe<TNode> RemoveByKey(TKey key)
        {
            if (collection.Remove(key, out TNode node))
                return node;

            return Maybe<TNode>.None;  
        }

        public bool RemoveNode(TNode node)
        {
            return GetNodeKey(node).Map(key => RemoveByKey(key).IsSome).Raw;
        }

        public bool ContainsKey(TKey key)
        {
            return collection.ContainsKey(key);
        }

        public bool ContainsNode(TNode node)
        {
            return collection.ContainsValue(node);
        }

        public Result<TNode> GetNode(TKey key)
        {
            if (!collection.TryGetValue(key, out TNode node))
                return (default!, new CCException($"Not found node with key: {key}"));

            return (node, null);
        }

        public Maybe<TKey> GetNodeKey(TNode value)
        {
            return collection.FirstOrDefault(pair => pair.Value!.Equals(value)).Key;
        }

        public void Clear()
        {
            collection.Clear();
            clearSubj?.OnNext(Unit.Default);
        }

        public IObservable<KeyValuePair<TKey, TNode>> ObserveAddNode()
        {
            addNodeSubj ??= new Subject<KeyValuePair<TKey, TNode>>();

            return addNodeSubj;
        }

        public IObservable<KeyValuePair<TKey, TNode>> ObserveRemoveNode()
        {
            removeNodeSubj ??= new Subject<KeyValuePair<TKey, TNode>>();

            return removeNodeSubj;
        }

        public IObservable<PreviousCurrentPair<KeyValuePair<TKey, TNode>>> ObserveReplaceNode()
        {
            replaceNodeSubj ??= new Subject<PreviousCurrentPair<KeyValuePair<TKey, TNode>>>();

            return replaceNodeSubj;
        }

        public IObservable<Unit> ObserveClear()
        {
            clearSubj ??= new Subject<Unit>();

            return clearSubj;
        }

        public IEnumerator<KeyValuePair<TKey, TNode>> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

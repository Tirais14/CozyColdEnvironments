using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using static CCEnvs.FuncLanguage.LangOperator;

#nullable enable
#pragma warning disable S1121
namespace CCEnvs.Unity.Collections
{
    public class NodeCollection<TKey, TNode>
        : INodeCollection<TKey, TNode>,
        IDisposable
    {
        protected readonly Dictionary<TKey, TNode> collection = new();
        protected readonly HashSet<GameObject> instantiatedGameObjects = new();
        private readonly Lazy<Dictionary<TNode, GameObject>> gameObjects = new(() => new Dictionary<TNode, GameObject>());
        private Subject<(KeyValuePair<TKey, TNode> node, GameObject go)>? addNodeByPrefabSubj;
        private Subject<(KeyValuePair<TKey, TNode> node, GameObject go)>? removeNodeByPrefabSubj;
        private Subject<Unit>? clearSubj;
        private Subject<KeyValuePair<TKey, TNode>>? addNodeSubj;
        private Subject<KeyValuePair<TKey, TNode>>? removeNodeSubj;

        public TNode this[TKey key] {
            get => collection[key];
            set
            {
                var t = collection[key];
                collection[key] = value;

                KeyValuePair<TKey, TNode> previous = KeyValuePair.Create(key, t);
                OnRemove(previous.Key, previous.Value);

                KeyValuePair<TKey, TNode> current = KeyValuePair.Create(key, value);
                OnAdd(current.Key, current.Value);
            }
        }

        public IReadOnlyDictionary<TNode, GameObject> GameObjects => gameObjects.Value;
        public int Count => collection.Count;
        public IEnumerable<TKey> Keys => collection.Keys;
        public IEnumerable<TNode> Nodes => collection.Values;
        public Func<IEnumerable<KeyValuePair<TKey, TNode>>, TNode, TKey>? KeyFactory { get; set; }
        public Action<TNode, TNode>? SetupClonedNodeAction { get; set; }
        public Maybe<GameObject> NodePrefab { get; set; }
        public Maybe<GameObject> gameObject { get; private set; }

        protected Func<IEnumerable<KeyValuePair<TKey, TNode>>, TNode, TKey> KeyFactoryUnsafe {
            get
            {
                if (KeyFactory is null)
                    CC.Throw.InvalidOperation("null", nameof(KeyFactory));

                return KeyFactory;
            }
        }

        protected GameObject NodePrefabUnsafe {
            get
            {
                if (NodePrefab.IsNone)
                    CC.Throw.InvalidOperation(NodePrefab, nameof(NodePrefab));

                return NodePrefab.GetValueUnsafe(); 
            }
        }

        public NodeCollection()
        {
        }

        public NodeCollection(int capacity)
        {
            collection = new Dictionary<TKey, TNode>(capacity);
        }

        public NodeCollection(IEnumerable<KeyValuePair<TKey, TNode>> nodes)
        {
            collection = new Dictionary<TKey, TNode>(nodes);
        }

        public NodeCollection(int nodeCount,
            GameObject nodePrefab)
        {
            NodePrefab = nodePrefab;
            collection = new Dictionary<TKey, TNode>(nodeCount);

            SetNodeCountByPrefab(nodeCount);
        }

        public bool AddNode(TKey key, TNode node)
        {
            if (collection.ContainsKey(key))
                return false;

            collection.Add(key, node);
            OnAdd(key, node);
            return true;
        }

        public Maybe<KeyValuePair<TKey, TNode>> AddNode(TNode node)
        {
            TKey key = KeyFactoryUnsafe.Invoke(this, node);
            var isAdded = AddNode(key, node);

            if (!isAdded)
                return Maybe<KeyValuePair<TKey, TNode>>.None;

            return KeyValuePair.Create(key, node);
        }

        public TNode AddNodeByPrefab(TKey key, GameObject prefab, Maybe<TNode> node = default)
        {
            return AddNodeByPrefabInternal(prefab, node: node).Value;
        }
        public TNode AddNodeByPrefab(TKey key, Maybe<TNode> node = default)
        {
            return AddNodeByPrefab(key, NodePrefabUnsafe, node);
        }

        public KeyValuePair<TKey, TNode> AddNodeByPrefab(GameObject prefab,
            Maybe<TNode> node = default)
        {
            return AddNodeByPrefabInternal(prefab, node: node);
        }
        public KeyValuePair<TKey, TNode> AddNodeByPrefab(Maybe<TNode> node = default)
        {
            return AddNodeByPrefab(NodePrefabUnsafe, node);
        }

        public KeyValuePair<TKey, TNode>[] AddNodeCount<T>(int count) where T : TNode, new()
        {
            var nodes = new List<KeyValuePair<TKey, TNode>>(count);
            KeyValuePair<TKey, TNode> node;
            for (int i = 0; i < count; i++)
            {
                node = AddNode(new T()).Map(node => KeyValuePair.Create(node.Key, node.Value.As<TNode>())).GetValue();
                nodes.Add(node);
            }

            return nodes.ToArray();
        }

        public KeyValuePair<TKey, TNode>[] AddNodeCountByPrefab(int count, GameObject prefab)
        {
            CC.Guard.IsNotNull(prefab, nameof(prefab));

            var nodes = new List<KeyValuePair<TKey, TNode>>(count);
            KeyValuePair<TKey, TNode> node;
            for (int i = 0; i < count; i++)
            {
                node = AddNodeByPrefab(prefab);
                nodes.Add(node);
            }

            return nodes.ToArray();
        }
        public KeyValuePair<TKey, TNode>[] AddNodeCountByPrefab(int count)
        {
            return AddNodeCountByPrefab(count, NodePrefabUnsafe);
        }

        public KeyValuePair<TKey, TNode>[] SetNodeCount<T>(int count) where T : TNode, new()
        {
            var delta = count - Count;

            if (delta < 0)
                return RemoveNodeCount(Math.Abs(delta));
            else if (delta > 0)
                return AddNodeCount<T>(delta);

            return Array.Empty<KeyValuePair<TKey, TNode>>();
        }

        public KeyValuePair<TKey, TNode>[] SetNodeCountByPrefab(int count, GameObject prefab)
        {
            var delta = count - Count;

            if (delta < 0)
                return RemoveNodeCount(Math.Abs(delta));
            else if (delta > 0)
                return AddNodeCountByPrefab(delta);

            return Array.Empty<KeyValuePair<TKey, TNode>>();
        }
        public KeyValuePair<TKey, TNode>[] SetNodeCountByPrefab(int count)
        {
            return SetNodeCountByPrefab(count, NodePrefabUnsafe);
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
            if (!collection.Remove(key, out TNode node))
                return default!;

            collection.Remove(key);
            OnRemove(key, node);
            return node;
        }

        public bool RemoveNode(TNode node)
        {
            return GetNodeKey(node).Map(key => RemoveByKey(key).IsSome).Raw;
        }

        public KeyValuePair<TKey, TNode>[] RemoveNodeCount(int count)
        {
            count = Math.Min(Count, count);
            var nodes = new List<KeyValuePair<TKey, TNode>>(count);

            for (int i = 0; i < count; i++)
                nodes.Add(RemoveLast().GetValueUnsafe());

            return nodes.ToArray();
        }

        public Maybe<KeyValuePair<TKey, TNode>> RemoveLast()
        {
            TKey key;
            if (Count <= 0
                ||
                !RemoveByKey(key = Keys.Last()).TryGetValue(out var node))
            {
                return Maybe<KeyValuePair<TKey, TNode>>.None;
            }

            return KeyValuePair.Create(key, node);
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
            foreach (var key in Keys.ToArray())
                RemoveByKey(key);

            clearSubj?.OnNext(Unit.Default);
        }

        public void Dispose() => Dispose(true);

        private bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                addNodeByPrefabSubj?.Dispose();
                removeNodeByPrefabSubj?.Dispose();
                clearSubj?.Dispose();
                addNodeSubj?.Dispose();
                removeNodeSubj?.Dispose();
            }

            disposed = true;
        }

        public IObservable<(KeyValuePair<TKey, TNode> node, GameObject go)> ObserveAddNodeByPrefab()
        {
            addNodeByPrefabSubj ??= new Subject<(KeyValuePair<TKey, TNode> node, GameObject go)>();

            return addNodeByPrefabSubj;
        }

        public IObservable<(KeyValuePair<TKey, TNode> node, GameObject go)> ObserveRemoveNodeWithGameObject()
        {
            removeNodeByPrefabSubj = new Subject<(KeyValuePair<TKey, TNode> node, GameObject go)>();

            return removeNodeByPrefabSubj;
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

        public IObservable<Unit> ObserveClear()
        {
            clearSubj ??= new Subject<Unit>();

            return clearSubj;
        }

        public IEnumerator<KeyValuePair<TKey, TNode>> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        protected virtual TNode GetComponentFromGameObject(GameObject go)
        {
            return go.QueryTo().ByChildren().Model<TNode>().Strict();
        }

        protected virtual void OnAdd(TKey key, TNode node)
        {
            if (addNodeByPrefabSubj is not null
                &&
                node is IGameObjectBindable goBindable
                &&
                goBindable.gameObject.TryGetValue(out GameObject? go))
            {
                addNodeByPrefabSubj.OnNext((KeyValuePair.Create(key, node), go));
            }
            else
                addNodeSubj?.OnNext(KeyValuePair.Create(key, node));
        }

        protected virtual void OnRemove(TKey key, TNode node)
        {
            if (GameObjects.TryGetValue(node, out GameObject go))
            {
                removeNodeByPrefabSubj?.OnNext((KeyValuePair.Create(key, node), go));

                if (instantiatedGameObjects.Contains(go))
                    Object.Destroy(go);
            }
            else
                removeNodeSubj?.OnNext(KeyValuePair.Create(key, node));
        }

        private KeyValuePair<TKey, TNode> AddNodeByPrefabInternal(
            GameObject prefab,
            Maybe<TNode> node = default,
            Maybe<TKey> key = default)
        {
            CC.Guard.IsNotNull(prefab, nameof(prefab));

            GameObject go = GetOrInstantiateNodeGameObject(node, prefab);
            TNode newNode = GetComponentFromGameObject(go);
            key = key.GetValue(() => KeyFactoryUnsafe.Invoke(this, newNode));

            if (node.IsSome
                &&
                SetupClonedNodeAction is not null)
            {
                SetupClonedNodeAction(node.GetValueUnsafe(), newNode);
            }

            if (AddNode(key.GetValueUnsafe(), newNode))
                gameObjects.Value.Add(newNode, go);

            return KeyValuePair.Create(key.GetValueUnsafe(), newNode);
        }

        private GameObject GetOrInstantiateNodeGameObject(Maybe<TNode> node, GameObject prefab)
        {
            if (node.IsSome
                &&
                node.GetValueUnsafe() is IGameObjectBindable goBindable)
            {
                if (goBindable.gameObject.IsSome)
                    return goBindable.gameObject.GetValueUnsafe();
                else
                {
                    GameObject go = Object.Instantiate(prefab);
                    instantiatedGameObjects.Add(go);

                    var isBinded = goBindable.BindGameObject(go);

                    if (!isBinded)
                        this.PrintWarning("Binding failured.");

                    return go;
                }
            }
            else
                return Object.Instantiate(prefab);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

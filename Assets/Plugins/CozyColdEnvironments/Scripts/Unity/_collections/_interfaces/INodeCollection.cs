using CCEnvs.FuncLanguage;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public interface INodeCollection<TKey, TNode> 
        : IEnumerable<KeyValuePair<TKey, TNode>>,
        IGameObjectBindable
    {
        TNode this[TKey key] { get; set; }

        IReadOnlyDictionary<TNode, GameObject> GameObjects { get; }
        int Count { get; }
        IEnumerable<TKey> Keys { get; }
        IEnumerable<TNode> Nodes { get; }
        Maybe<GameObject> NodePrefab { get; set; }
        Func<IEnumerable<KeyValuePair<TKey, TNode>>, TNode, TKey>? KeyFactory { get; set; }

        Result<TNode> GetNode(TKey key);

        bool AddNode(TKey key, TNode node);
        Maybe<KeyValuePair<TKey, TNode>> AddNode(TNode node);

        TNode AddNodeByPrefab(TKey key, GameObject prefab, Maybe<TNode> node = default);
        TNode AddNodeByPrefab(TKey key, Maybe<TNode> node = default);
        KeyValuePair<TKey, TNode> AddNodeByPrefab(GameObject prefab, Maybe<TNode> node = default);
        KeyValuePair<TKey, TNode> AddNodeByPrefab(Maybe<TNode> node = default);

        KeyValuePair<TKey, TNode>[] AddNodeCount<T>(int count) where T : TNode, new();

        KeyValuePair<TKey, TNode>[] AddNodeCountByPrefab(int count, GameObject prefab);
        KeyValuePair<TKey, TNode>[] AddNodeCountByPrefab(int count);

        KeyValuePair<TKey, TNode>[] SetNodeCount<T>(int count) where T : TNode, new();

        KeyValuePair<TKey, TNode>[] SetNodeCountByPrefab(int count, GameObject prefab);
        KeyValuePair<TKey, TNode>[] SetNodeCountByPrefab(int count);

        Maybe<TNode> RemoveByKey(TKey key);

        bool RemoveNode(TNode node);

        KeyValuePair<TKey, TNode>[] RemoveNodeCount(int count);

        Maybe<KeyValuePair<TKey, TNode>> RemoveLast();

        Maybe<TKey> GetNodeKey(TNode value);

        bool ContainsKey(TKey key);
        bool ContainsNode(TNode node);

        void Clear();

        IObservable<(KeyValuePair<TKey, TNode> node, GameObject go)> ObserveAddNodeByPrefab();

        IObservable<(KeyValuePair<TKey, TNode> node, GameObject go)> ObserveRemoveNodeWithGameObject();

        IObservable<KeyValuePair<TKey, TNode>> ObserveAddNode();

        IObservable<KeyValuePair<TKey, TNode>> ObserveRemoveNode();

        IObservable<Unit> ObserveClear();
    }
}

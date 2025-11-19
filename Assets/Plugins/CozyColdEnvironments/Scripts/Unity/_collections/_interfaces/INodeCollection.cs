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

        IEnumerable<TKey> Keys { get; }
        IEnumerable<TNode> Nodes { get; }
        Maybe<GameObject> NodePrefab { get; set; }
        Maybe<Func<IEnumerable<KeyValuePair<TKey, TNode>>, TNode, TKey>> KeyFactory { get; set; }

        Result<TNode> GetNode(TKey key);

        bool AddNode(TNode node);
        bool AddNode(TKey key, TNode node);
        TNode AddNodeByPrefab(TKey key, TNode node, GameObject prefab);
        TNode AddNodeByPrefab(TKey key, TNode node);

        Maybe<TNode> RemoveByKey(TKey key);

        bool RemoveNode(TNode node);

        Maybe<TKey> GetNodeKey(TNode value);

        bool ContainsKey(TKey key);
        bool ContainsNode(TNode item);

        void Clear();

        IObservable<KeyValuePair<TKey, TNode>> ObserveAddNode();

        IObservable<KeyValuePair<TKey, TNode>> ObserveRemoveNode();

        IObservable<PreviousCurrentPair<KeyValuePair<TKey, TNode>>> ObserveReplaceNode();

        IObservable<Unit> ObserveClear();
    }
}

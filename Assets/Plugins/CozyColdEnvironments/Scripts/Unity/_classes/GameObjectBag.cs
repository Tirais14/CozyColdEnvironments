using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using ZLinq;
using static CCEnvs.Unity.UI.Elements.IGameObjectBag;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    public class GameObjectBag : ViewElement, IGameObjectBag
    {
        protected readonly ReactiveCollection<GameObject> collection = new();

        public Settings settings { get; set; } = Settings.Default;
        public int Count => collection.Count;

        GameObject IReadOnlyReactiveCollection<GameObject>.this[int index] {
            get => collection[index];
        }

        protected override void Start()
        {
            base.Start();
            //Refresh();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            collection.Dispose();
        }

        public void Refresh()
        {
            this.FindFor()
                .ChildrenGameObjects()
                .ZL()
                .Where(go => !Contains(go))
                .ForEach(Add);
        }

        public void Add(GameObject item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            collection.Add(item);
            OnAdd(item);
        }

        public void Clear()
        {
            foreach (var go in collection)
                OnRemove(go);

            collection.Clear();
        }

        public bool Contains(GameObject item) => collection.Contains(item);

        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

#pragma warning disable UNT0029
        public bool Remove(GameObject item)
        {
            if (item is null)
                return false;

            if (collection.Remove(item))
            {
                OnRemove(item);
                return true;
            }

            return false;
        }
#pragma warning restore UNT0029

        public IObservable<CollectionAddEvent<GameObject>> ObserveAdd()
        {
            return collection.ObserveAdd();
        }

        public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
        {
            return collection.ObserveCountChanged(notifyCurrentCount);
        }

        public IObservable<CollectionMoveEvent<GameObject>> ObserveMove()
        {
            return collection.ObserveMove();
        }

        public IObservable<CollectionRemoveEvent<GameObject>> ObserveRemove()
        {
            return collection.ObserveRemove();
        }

        public IObservable<CollectionReplaceEvent<GameObject>> ObserveReplace()
        {
            return collection.ObserveReplace();
        }

        public IObservable<Unit> ObserveReset()
        {
            return collection.ObserveReset();
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        protected virtual void OnAdd(GameObject go)
        {
            Transform goTransform = go.transform;

            if (settings.IsFlagSetted(Settings.ReparentByRootMarker))
                goTransform = go.FindFor().RootTransform();

            goTransform.SetParent(transform);

            if (settings.IsFlagSetted(Settings.ActivateOnAdd))
                goTransform.gameObject.SetActive(true);

            go.OnDestroyAsObservable()
              .Subscribe(_ => collection.Remove(go))
              .AddTo(go);
        }

        protected virtual void OnRemove(GameObject go)
        {
            if (go == null)
                return;

            if (settings.IsFlagSetted(Settings.ReparentByRootMarker))
            {
                var root = go.FindFor().RootRaw();

                root.IfRight(x =>
                {
                    if (settings.IsFlagSetted(Settings.DestroyOnRemove))
                        Destroy(x.gameObject);
                    else
                        x.transform.SetParent(null);

                    if (settings.IsFlagSetted(Settings.DeactivateOnRemove))
                        x.gameObject.SetActive(false);
                });
            }
            else
            {
                if (settings.IsFlagSetted(Settings.DestroyOnRemove))
                    Destroy(go);
                else
                    go.transform.SetParent(null);

                if (settings.IsFlagSetted(Settings.DeactivateOnRemove)
                    &&
                    go != null)
                {
                    go.SetActive(false);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

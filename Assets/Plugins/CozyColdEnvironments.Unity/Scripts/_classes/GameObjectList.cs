using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CCEnvs.Unity.Components;
using ObservableCollections;
using R3;
using R3.Collections;
using R3.Triggers;
using UnityEngine;
using static CCEnvs.Unity.UI.IGameObjectBag;

#nullable enable
namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    public class GameObjectList : CCBehaviour, IGameObjectBag
    {
        protected readonly ObservableList<GameObject> collection = new();

        public event NotifyCollectionChangedEventHandler<GameObject>? CollectionChanged;

        public Settings settings { get; set; } = Settings.Default;
        public int Count => collection.Count;
        public object SyncRoot => collection.SyncRoot;

        public GameObject this[int index] {
            get => collection[index];
            set => collection[index] = value;
        }

        protected override void Awake()
        {
            base.Awake();
            Setup();
        }

        protected override void Start()
        {
            base.Start();
            Refresh();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            Refresh();
        }

        public void Refresh()
        {
            collection.Clear();
            foreach (var child in this.Q().ChildrenGameObjects())
                collection.Add(child);
        }

        public void Add(GameObject item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            collection.Add(item);
            OnAdd(item);
            Refresh();
        }

        public void Insert(int index, GameObject item)
        {
            collection.Insert(index, item);
            OnAdd(item);
            Refresh();
        }

        public void Clear()
        {
            foreach (var go in collection.ToArray())
                OnRemove(go);

            collection.Clear();
        }

        public bool Contains(GameObject item) => collection.Contains(item);

        public void Move(int oldIndex, int newIndex)
        {
            collection.Move(oldIndex, newIndex);
        }

        public int IndexOf(GameObject item)
        {
            return collection.IndexOf(item);
        }

        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(GameObject item)
        {
#pragma warning disable UNT0029
            if (item is null)
                return false;
#pragma warning restore UNT0029

            if (collection.Remove(item))
            {
                OnRemove(item);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            collection.RemoveAt(index);
        }

        public ISynchronizedView<GameObject, TView> CreateView<TView>(System.Func<GameObject, TView> transform)
        {
            return collection.CreateView(transform);
        }

        public Observable<CollectionAddEvent<GameObject>> ObserveAdd()
        {
            return collection.ObserveAdd();
        }

        public Observable<int> ObserveCountChanged(bool notifyCurrentCount = false)
        {
            return collection.ObserveCountChanged(notifyCurrentCount);
        }

        public Observable<CollectionMoveEvent<GameObject>> ObserveMove()
        {
            return collection.ObserveMove();
        }

        public Observable<CollectionRemoveEvent<GameObject>> ObserveRemove()
        {
            return collection.ObserveRemove();
        }

        public Observable<CollectionReplaceEvent<GameObject>> ObserveReplace()
        {
            return collection.ObserveReplace();
        }

        public Observable<CollectionResetEvent<GameObject>> ObserveReset()
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

            if (settings.HasFlagT(Settings.ReparentByRootMarker))
                goTransform = go.QueryTo().RootTransform();

            goTransform.SetParent(transform);

            if (settings.HasFlagT(Settings.ActivateOnAdd))
                goTransform.gameObject.SetActive(true);

            go.OnDestroyAsObservable()
              .Subscribe((@this: this, go),
              static (_, input) =>
              {
                  input.@this.collection.Remove(input.go);
              })
              .AddDisposableTo(this);
        }

        protected virtual void OnRemove(GameObject go)
        {
            if (go == null)
                return;

            if (settings.HasFlagT(Settings.ReparentByRootMarker))
            {
                var root = go.QueryTo().RootRaw();

                root.IfRight(x =>
                {
                    if (settings.HasFlagT(Settings.DestroyOnRemove))
                        Destroy(x.gameObject);
                    else
                        x.transform.SetParent(null);

                    if (settings.HasFlagT(Settings.DeactivateOnRemove))
                        x.gameObject.SetActive(false);
                });
            }
            else
            {
                if (settings.HasFlagT(Settings.DestroyOnRemove))
                    Destroy(go);
                else
                    go.transform.SetParent(null);

                if (settings.HasFlagT(Settings.DeactivateOnRemove)
                    &&
                    go != null)
                {
                    go.SetActive(false);
                }
            }
        }

        private void Setup()
        {
            collection.ObserveReplace()
                .Subscribe(this,
                static (ev, @this) =>
                {
                    @this.OnRemove(ev.OldValue);
                    @this.OnAdd(ev.NewValue);
                })
                .AddDisposableTo(this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

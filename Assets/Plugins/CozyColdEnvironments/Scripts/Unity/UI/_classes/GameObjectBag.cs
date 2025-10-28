using CCEnvs.FuncLanguage;
using SuperLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    public class GameObjectBag : ViewElement, IGameObjectBag
    {
        protected Maybe<ReactiveCollection<GameObject>> collection;

        public bool DestroyOnRemove { get; set; }
        public int Count => collection.Map(x => x.Count).Access();

        protected override bool showOnStart => true;

        protected ReactiveCollection<GameObject> Inner {
            get
            {
                return collection.IfNone(() => { collection = new ReactiveCollection<GameObject>(); })
                            .Access(collection.Access())!;
            }
        }

        GameObject IReadOnlyReactiveCollection<GameObject>.this[int index] {
            get
            {
                return collection.Match(
                       x => x[index],
                       () => CC.Throw.IndexOutOfRange(index).As<GameObject>())
                   .AccessUnsafe();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            Show();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            collection.IfSome(x => x.Dispose());
        }

        public void Add(GameObject item)
        {
            CC.Guard.NullArgument(item, nameof(item));

            Inner.Add(item);
            OnAdd(item);
        }

        public void Clear()
        {
            collection.IfSome(x => x.ForEach(OnRemove))
                 .IfSome((x) => x.Clear());
        }

        public bool Contains(GameObject item) => Inner.Contains(item);

        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            collection.IfSome(x => x.CopyTo(array, arrayIndex));
        }

        public bool Remove(GameObject item)
        {
            if (item == null)
                return false;

            return collection.Map(x => x.Remove(item))
                        .IfSome(_ => OnRemove(item))
                        .Access();
        }

        public IObservable<CollectionAddEvent<GameObject>> ObserveAdd()
        {
            return Inner.ObserveAdd();
        }

        public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
        {
            return Inner.ObserveCountChanged(notifyCurrentCount);
        }

        public IObservable<CollectionMoveEvent<GameObject>> ObserveMove()
        {
            return Inner.ObserveMove();
        }

        public IObservable<CollectionRemoveEvent<GameObject>> ObserveRemove()
        {
            return Inner.ObserveRemove();
        }

        public IObservable<CollectionReplaceEvent<GameObject>> ObserveReplace()
        {
            return Inner.ObserveReplace();
        }

        public IObservable<Unit> ObserveReset()
        {
            return Inner.ObserveReset();
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return collection.Map(x => x.GetEnumerator())
                        .Access(Enumerable.Empty<GameObject>().GetEnumerator())!;
        }

        protected void OnAdd(GameObject go)
        {
            go.transform.SetParent(transform);

            go.OnDestroyAsObservable()
              .Subscribe(_ => Inner.Remove(go))
              .AddTo(go);
        }

        protected void OnRemove(GameObject go)
        {
            if (DestroyOnRemove)
                Destroy(go);

            go.transform.SetParent(null);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

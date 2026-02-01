using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using UnityEngine;
using static UnityEditor.Progress;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public class ComponentList<T> : CCBehaviour
        where T : class
    {
        private bool isChildsCollecting;

        public ObservableHashSet<T> Value { get; } = new();

        public bool IsDestroyOnRemove { get; private set; }
        public bool IsDestroyByGameObject { get; private set;  }

        protected override void Start()
        {
            base.Start();
            BindComponentCollection();
            CollectChildsAsync().Forget();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            CollectChildsAsync().Forget();
        }

        public ComponentList<T> SetDestroyOnRemove(bool state)
        {
            IsDestroyOnRemove = state;

            return this;
        }

        public ComponentList<T> SetDestroyByGameObject(bool state)
        {
            IsDestroyByGameObject = state;

            return this;
        }

        private void OnAdd(T item)
        {
            var cmp = item.To<Component>();

            if (cmp.transform.parent == cTransform.Value)
                return;

            cmp.transform.SetParent(cTransform.Value);
        }

        private void OnRemove(T item)
        {
            var cmp = item.To<Component>();

            if (IsDestroyOnRemove)
            {
                if (IsDestroyByGameObject)
                {
                    Destroy(cmp.gameObject);
                    return;
                }

                Destroy(cmp);
                return;
            }

            cmp.transform.SetParent(null);
        }

        private void BindComponentCollection()
        {
            Value.ObserveAdd(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) => @this.OnAdd(item))
                .RegisterDisposableTo(this);

            Value.ObserveRemove(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) => @this.OnRemove(item))
                .RegisterDisposableTo(this);
        }

        private async UniTaskVoid CollectChildsAsync()
        {
            if (isChildsCollecting)
                return;

            isChildsCollecting = true;

            await UniTask.WaitForEndOfFrame(cancellationToken: destroyCancellationToken);

            Value.Clear();

            var cmps = this.Q().FromChildrens().IncludeInactive().Models<T>();

            foreach (var cmp in cmps)
                Value.Add(cmp);

            isChildsCollecting = false;
        }
    }
}

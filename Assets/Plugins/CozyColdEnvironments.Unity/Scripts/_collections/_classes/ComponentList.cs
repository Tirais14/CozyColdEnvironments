using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Collections
{
    public class ComponentList<T> : CCBehaviour
        where T : class
    {
        private bool isChildsCollecting;
        private bool isInternalClear;

        public ObservableHashSet<T> Value { get; } = new();

        public bool IsDestroyOnRemove { get; private set; }
        public bool IsDestroyByGameObject { get; private set; }

        public Type? TypeFilter { get; private set; }

        protected override void Start()
        {
            base.Start();

            BindComponentAdd();
            BindComponentRemove();
            BindComponentsClear();

            OnTransformChildrenChanged();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            CollectChildsAsync().ForgetByPrintException();
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

        public ComponentList<T> SetTypeFilter(Type? type)
        {
            TypeFilter = type;

            return this;
        }

        private void OnComponentAdd(T item)
        {
            var cmp = item.To<Component>();

            if (cmp.transform.parent == cTransform)
                return;

            cmp.transform.SetParent(cTransform);
        }

        private void OnComponentRemove(T item)
        {
            var cmp = item.To<Component>();

            if (IsDestroyOnRemove)
            {
                if (IsDestroyByGameObject)
                {
                    Destroy(cmp.gameObject);
                    return;
                }

                cmp.transform.SetParent(null);
                Destroy(cmp);
                return;
            }

            cmp.transform.SetParent(null);
        }

        private void OnComponentsClear()
        {
            GetChilds().ForEach(OnComponentRemove);
        }

        private void BindComponentAdd()
        {
            Value.ObserveAdd(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) => @this.OnComponentAdd(item))
                .AddDisposableTo(this);
        }

        private void BindComponentRemove()
        {
            Value.ObserveRemove(destroyCancellationToken)
                .Select(static ev => ev.Value)
                .Subscribe(this,
                static (item, @this) => @this.OnComponentRemove(item))
                .AddDisposableTo(this);
        }

        private void BindComponentsClear()
        {
            Value.ObserveClear(destroyCancellationToken)
                .Where(this, 
                static (_, @this) => !@this.isInternalClear)
                .Subscribe(this,
                static (_, @this) => @this.OnComponentsClear())
                .AddDisposableTo(this);
        }

        private IEnumerable<T> GetChilds()
        {
            if (TypeFilter is not null)
            {
                return this.Q()
                    .FromChildrens()
                    .IncludeInactive()
                    .NotRecursive()
                    .Models(TypeFilter, includeComponents: true)
                    .OfType<T>();
            }
            else
            {
                return this.Q()
                    .FromChildrens()
                    .NotRecursive()
                    .IncludeInactive()
                    .Models<T>();
            }
        }

        private async UniTask CollectChildsAsync()
        {
            if (isChildsCollecting)
                return;

            isChildsCollecting = true;

            try
            {
                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, cancellationToken: destroyCancellationToken);

                isInternalClear = true;

                Value.Clear();

                isInternalClear = false;

                foreach (var cmp in GetChilds())
                    Value.Add(cmp);
            }
            finally
            {
                isChildsCollecting = false;
            }
        }
    }

    public class ComponentList : ComponentList<Component>
    {

    }
}

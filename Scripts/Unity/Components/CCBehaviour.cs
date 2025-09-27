using CCEnvs.Unity.ComponentSetter;
using CCEnvs.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable

namespace CCEnvs.Unity.Components
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class CCBehaviour : MonoBehaviour
    {
        //public static event Action<MonoX>? OnInstantiated;

        private LazyCC<ComponentCache> baseCache = null!;

        protected event Action? onEndFirstFrame;

        public ComponentCache BaseCache => baseCache.Value;

        protected virtual void OnAwake()
        { 

        }

        protected virtual void OnStart()
        { 
        }

        public bool TryGetComponentInChildren(Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            return ComponentGetComponentExtensions.TryGetComponentInChildren(
                this,
                type,
                includeInactive,
                out result);
        }
        public bool TryGetComponentInChildren(Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return ComponentGetComponentExtensions.TryGetComponentInChildren(
                this,
                type,
                out result);
        }

        public bool TryGetComponentInChildren<T>(bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            return ComponentGetComponentExtensions.TryGetComponentInChildren(
                this,
                includeInactive,
                out result);
        }
        public bool TryGetComponentInChildren<T>([NotNullWhen(true)] out T? result)
        {
            return ComponentGetComponentExtensions.TryGetComponentInChildren(
                this,
                out result);
        }

        public bool TryGetComponentInParent(Type type,
            bool includeInactive,
            [NotNullWhen(true)] out Component? result)
        {
            return ComponentGetComponentExtensions.TryGetComponentInParent(
                this,
                type,
                includeInactive,
                out result);
        }
        public bool TryGetComponentInParent(Type type,
            [NotNullWhen(true)] out Component? result)
        {
            return ComponentGetComponentExtensions.TryGetComponentInParent(
                this,
                type,
                out result);
        }

        public bool TryGetComponentInParent<T>(bool includeInactive,
            [NotNullWhen(true)] out T? result)
        {
            return ComponentGetComponentExtensions.TryGetComponentInParent(
                this,
                includeInactive,
                out result);
        }
        public bool TryGetComponentInParent<T>([NotNullWhen(true)] out T? result)
        {
            return ComponentGetComponentExtensions.TryGetComponentInParent(
                this,
                out result);
        }

        protected bool TryLockDestroyOnLoad()
        {
            if (transform.parent == null)
            {
                DontDestroyOnLoad(this);

                return true;
            }

            return false;
        }

        protected void Awake()
        {
            //OnInstantiated?.Invoke(this);

            baseCache = new LazyCC<ComponentCache>(() => new ComponentCache(transform, gameObject));

            //Sets component fields and props marked by specical attribute
            GetComponentByAttributeHelper.SetMembers(this);

            OnAwake();
        }

        protected void Start()
        {
            OnStart();

            if (onEndFirstFrame is not null)
                _ = OnEndFirstFrameInvokerAsync();

            //Checks field and props marked by RequiredAttribute
            MemberValidator.ValidateInstance(this);
        }

        private async UniTaskVoid OnEndFirstFrameInvokerAsync()
        {
            await UniTask.WaitForEndOfFrame();

            onEndFirstFrame!();
        }
    }
}
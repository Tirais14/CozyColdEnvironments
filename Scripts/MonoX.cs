using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UTIRLib.ComponentSetter;
using UTIRLib.Unity;
using UTIRLib.Utils;

#nullable enable

namespace UTIRLib
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class MonoX : MonoBehaviour
    {
        protected event Action? onEndFirstFrame;

        protected virtual void OnAwake()
        { }

        protected virtual void OnStart()
        { }

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

        protected void Message(object message)
        {
            Debug.Log(message, this);
        }

        protected void MessageFormat(string message, params object[] args)
        {
            Debug.LogFormat(this, message, args);
        }

        protected void Warning(object message)
        {
            Debug.LogWarning(message, this);
        }

        protected void WarningFormat(string message, params object[] args)
        {
            Debug.LogWarningFormat(this, message, args);
        }

        protected void Error(object message)
        {
            Debug.LogError(message, this);
        }

        protected void ErrorFormat(string message, params object[] args)
        {
            Debug.LogErrorFormat(this, message, args);
        }

        protected void LogException(Exception exception)
        {
            Debug.LogException(exception, this);
        }

        protected void Awake()
        {
            //Sets component fields and props marked by specical attribute
            GetComponentByAttributeHelper.SetMembers(this);

            OnAwake();

            //Checks field and props marked by RequiredAttribute
            MemberValidator.ValidateInstance(this);
        }

        protected void Start()
        {
            OnStart();

            if (onEndFirstFrame is not null)
                _ = OnEndFirstFrameInvokerAsync();
        }

        private async UniTaskVoid OnEndFirstFrameInvokerAsync()
        {
            await UniTask.WaitForEndOfFrame();

            onEndFirstFrame!();
        }
    }
}
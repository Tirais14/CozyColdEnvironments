using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UTIRLib.ComponentSetter;
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

        protected Transform[] GetChilds()
        {
            if (transform.childCount == 0)
                return Array.Empty<Transform>();

            int childCount = transform.childCount;
            var childs = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
                childs[i] = transform.GetChild(i);

            return childs;
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

        protected T? AddComponent<T>(Type type) where T : Component
        {
            return ComponentHelper.AddComponent<MonoBehaviour, T>(this, type);
        }

        protected T AddComponent<T>() where T : Component
        {
            return ComponentHelper.AddComponent<MonoBehaviour, T>(this);
        }

        protected void AddComponent<T>([NotNull] ref T? value) where T : Component
        {
            ComponentHelper.AddComponent<MonoBehaviour, T>(this, ref value);
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
            ComponentContainableMemberSetHelper.SetMembers(this);

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
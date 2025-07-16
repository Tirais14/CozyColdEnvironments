using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UTIRLib.Injector;
using UTIRLib.Utils;

#nullable enable

namespace UTIRLib
{
    public class MonoX : MonoBehaviour
    {
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        protected event Action? onEndFirstFrame;

        protected virtual void OnAwake()
        { }

        protected virtual void OnStart()
        { }

        protected Transform[] GetChilds()
        {
            if (transform.childCount == 0)
            {
                return Array.Empty<Transform>();
            }

            int childCount = transform.childCount;
            var childs = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                childs[i] = transform.GetChild(i);
            }

            return childs;
        }

        protected void DontDestroyOnLoad()
        {
            if (transform.parent == null)
            {
                DontDestroyOnLoad(this);
            }
        }

        protected T? AddComponent<T>(Type type) where T : Component =>
            ComponentHelper.AddComponent<MonoBehaviour, T>(this, type);

        protected T AddComponent<T>() where T : Component =>
            ComponentHelper.AddComponent<MonoBehaviour, T>(this);

        protected void AddComponent<T>([NotNull] ref T? value) where T : Component =>
            ComponentHelper.AddComponent<MonoBehaviour, T>(this, ref value);

        protected void Message(object message) => Debug.Log(message, this);

        protected static void MessageFormat(string message, params object[] args) => Debug.LogFormat(message, args);

        protected void Warning(object message) => Debug.LogWarning(message, this);

        protected static void WarningFormat(string message, params object[] args) => Debug.LogWarningFormat(message, args);

        protected void Error(object message) => Debug.LogError(message, this);

        protected static void ErrorFormat(string message, params object[] args) => Debug.LogErrorFormat(message, args);

        protected void LogException(Exception exception) => Debug.LogException(exception, this);

        protected void Awake()
        {
            ComponentInjector.Inject(this); //Sets component fields and props marked by GetComponentAttribute

            OnAwake();

            MemberValidator.ValidateInstance(this); //Checks field and props marked by RequiredAttribute
        }

        protected void Start()
        {
            OnStart();

            if (onEndFirstFrame is not null)
            {
                StartCoroutine(EndOfFirstFrameObserver());
            }
        }

        private IEnumerator EndOfFirstFrameObserver()
        {
            yield return new WaitForEndOfFrame();
            onEndFirstFrame!();
        }
    }
}
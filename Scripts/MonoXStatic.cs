#nullable enable

using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UTIRLib.Diagnostics;

namespace UTIRLib
{
    public abstract class MonoXStatic : MonoX
    {
        private static Transform? parent = null!;

        /// <summary>
        /// Parent of any instantiated <see cref="MonoXStatic"/>
        /// </summary>
        public static Transform? Parent {
            get => parent;
            set {
                parent = value;

                ReParentInstances();
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            if (parent.IsNull())
                parent = GetDefaultParent();
        }

        protected static T Create<T>()
            where T : Component
        {
            if (TryGetInstance<T>(out var instance))
            {
                TirLibDebug.Log($"{typeof(T)} already exists.",
                                true);

                return instance;
            }

            GameObject gameObject = new(typeof(T).Name){
                isStatic = true
            };

            gameObject.transform.parent = Parent;

            Transform thisParent = gameObject.transform;
            while (thisParent.parent != null)
                thisParent = thisParent.parent;

            DontDestroyOnLoad(thisParent.gameObject);

            return gameObject.AddComponent<T>();
        }

        private static Transform GetDefaultParent()
        {
            if (GameObject.Find("_Static") is GameObject go)
                return go.transform;

            GameObject empty = new("_Static"){
                isStatic = true
            };

            DontDestroyOnLoad(empty);

            return empty.transform;
        }

        private static bool TryGetInstance<T>([NotNullWhen(true)] out T? result)
            where T : Component
        {
            if (parent != null)
                result = parent.GetComponentInChildren<MonoXStatic>() as T;
            else
            {
                result = FindAnyObjectByType<T>();

                if (result != null && parent != null)
                    result.transform.parent = parent;
            }

            return result != null;
        }

        private static void ReParentInstances()
        {
            var instances = FindObjectsByType<MonoXStatic>(FindObjectsInactive.Include,
                                                           FindObjectsSortMode.None);
            int instancesCount = instances.Length;
            for (int i = 0; i < instancesCount; i++)
                instances[i].transform.parent = parent;
        }
    }

    public abstract class MonoXStatic<TThis> : MonoXStatic
        where TThis : Component
    {
        protected static LazyProperty<TThis> instance = new(Create<TThis>);
    }
}
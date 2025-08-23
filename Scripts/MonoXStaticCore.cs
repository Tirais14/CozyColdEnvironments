using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib
{
    [DefaultExecutionOrder(-10)]
    public sealed class MonoXStaticCore : MonoX
    {
        private static MonoXStaticCore instance = null!;

        private readonly Dictionary<Type, MonoXStatic> instances = new();

        protected override void OnAwake()
        {
            if (FindObjectsByType<MonoXStaticCore>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Any(x => x != this))
                throw new LogicException($"Cannot create more than one {nameof(MonoXStaticCore)}.");

            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += (_, _) => instances.Clear();
            base.OnAwake();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static MonoXStatic GetInstance(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (instance == null)
                GetOrCreateSelf();

            if (!instance!.instances.TryGetValue(type, out MonoXStatic? result))
                return instance.GetOrCreateStatic(type);
            if (result == null)
            {
                instance.instances.Remove(type);
                return instance.GetOrCreateStatic(type);
            }

            return result;
        }
        public static T GetInstance<T>()
            where T : MonoXStatic
        {
            return (T)GetInstance(typeof(T));
        }

        private static void GetOrCreateSelf()
        {
            if (FindAnyObjectByType<MonoXStaticCore>(FindObjectsInactive.Include)
                .Is<MonoXStaticCore>(out var found)
                )
            {
                instance = found;
                return;
            }

            var go = new GameObject("___StaticCore", typeof(MonoXStaticCore));
            instance = go.GetComponent<MonoXStaticCore>();
        }

        private MonoXStatic GetOrCreateStatic(Type type)
        {
            var value = (MonoXStatic?)FindAnyObjectByType(type);

            if (value == null)
                value = (MonoXStatic)gameObject.AddComponent(type);

            instances.Add(value.GetType(), value);

            return value;
        }
    }
}

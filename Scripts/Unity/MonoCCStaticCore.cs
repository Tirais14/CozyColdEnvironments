using CCEnvs.Diagnostics;
using CCEnvs.TypeMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    [DefaultExecutionOrder(-10)]
    public sealed class MonoCCStaticCore : MonoCC
    {
        private static MonoCCStaticCore instance = null!;

        private readonly Dictionary<Type, MonoCCStatic> instances = new();

        protected override void OnAwake()
        {
            if (FindObjectsByType<MonoCCStaticCore>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Any(x => x != this))
                throw new CCFrameworkException($"Cannot create more than one {nameof(MonoCCStaticCore)}.");

            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += (_, _) => instances.Clear();
            base.OnAwake();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static MonoCCStatic GetInstance(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (instance == null)
                GetOrCreateSelf();

            if (!instance!.instances.TryGetValue(type, out MonoCCStatic? result))
                return instance.GetOrCreateStatic(type);
            if (result == null)
            {
                instance.instances.Remove(type);
                return instance.GetOrCreateStatic(type);
            }

            return result;
        }
        public static T GetInstance<T>()
            where T : MonoCCStatic
        {
            return (T)GetInstance(typeof(T));
        }

        private static void GetOrCreateSelf()
        {
            if (FindAnyObjectByType<MonoCCStaticCore>(FindObjectsInactive.Include)
                .Is<MonoCCStaticCore>(out var found)
                )
            {
                instance = found;
                return;
            }

            var go = new GameObject("___StaticCore", typeof(MonoCCStaticCore));
            instance = go.GetComponent<MonoCCStaticCore>();
        }

        private MonoCCStatic GetOrCreateStatic(Type type)
        {
            var value = (MonoCCStatic?)FindAnyObjectByType(type);

            if (value == null)
                value = (MonoCCStatic)gameObject.AddComponent(type);

            instances.Add(value.GetType(), value);

            return value;
        }
    }
}

using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Attributes;
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
            Validate();
            Setup();
            CreateInstantClasses();

            base.OnAwake();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static MonoCCStatic GetInstance(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (instance == null)
                GetOrCreateSelf();

            if (!instance!.instances.TryGetValue(type, out MonoCCStatic? result)
                || result == null
                )
                return instance.GetInstanceOf(type);
            if (result == null)
            {
                var created = instance.GetInstanceOf(type);
                instance.instances[type] = created;

                return created;
            }

            return result;
        }
        public static T GetInstance<T>()
            where T : MonoCCStatic
        {
            return (T)GetInstance(typeof(T));
        }

        private void Validate()
        {
            if (FindObjectsByType<MonoCCStaticCore>(
                FindObjectsInactive.Include, FindObjectsSortMode.None).Any(x => x != this)
                )
                throw new CCFrameworkException($"Cannot create more than one {nameof(MonoCCStaticCore)}.");
        }

        private void Setup()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += (_, _) => instances.Clear();
        }

        private void CreateInstantClasses()
        {
            int instantCreatedClassCount = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsType<MonoCCStatic>())
                .Where(type => type.IsDefined<InstantCreationAttribute>(inherit: false))
                .ForEach(type => type.AsReflected()
                                     .Method(nameof(MonoCCStatic<MonoCCStatic>.TryInstantiateManual))
                                     .Invoke())
                .Length;

            CCDebug.PrintLog($"Instant created class count = {instantCreatedClassCount}.",
                             new DebugContext(this).Additive());
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

        private MonoCCStatic GetInstanceOf(Type type)
        {
            CC.Validate.Argument(type,
                                 nameof(type),
                                 !type.IsAbstract && !type.IsInterface,
                                 "Type is abstract and cannot be added.");
            var value = (MonoCCStatic?)FindAnyObjectByType(type);

            if (value == null)
                value = (MonoCCStatic)gameObject.AddComponent(type);

            if (instances.ContainsKey(type))
                instances[type] = value;
            else
                instances.Add(value.GetType(), value);

            return value;
        }
    }
}

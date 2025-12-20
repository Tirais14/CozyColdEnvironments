using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Attributes;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity
{
    [DefaultExecutionOrder(-10)]
    public sealed class CCBehaviourStaticKernel : CCBehaviour
    {
        private static CCBehaviourStaticKernel instance = null!;

        private readonly Dictionary<Type, CCBehaviourStatic> instances = new();

        protected override void Awake()
        {
            Validate();
            Setup();
            CreateInstantClasses();

            base.Awake();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static CCBehaviourStatic GetInstance(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (instance == null)
                GetOrCreateSelf();

            if (!instance!.instances.TryGetValue(type, out CCBehaviourStatic? result)
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
            where T : CCBehaviourStatic
        {
            return (T)GetInstance(typeof(T));
        }

        private void Validate()
        {
            if (FindObjectsByType<CCBehaviourStaticKernel>(
                FindObjectsInactive.Include, FindObjectsSortMode.None).Any(x => x != this)
                )
                throw new CCException($"Cannot create more than one {nameof(CCBehaviourStaticKernel)}.");
        }

        private void Setup()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += (_, _) => instances.Clear();
        }

        private void CreateInstantClasses()
        {
            int instantCreatedClassCount =
                (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                 select assembly.GetTypes() into types
                 from type in types
                 where type.IsType<CCBehaviourStatic>()
                 where type.IsDefined<InstantCreationAttribute>(inherit: false)
                 select type)
                 .CForEach(type => GetInstance(type))
                 .Length;

            CCDebug.Instance.PrintLog($"Instant created class count = {instantCreatedClassCount}.",
                             new DebugContext(this).Additive());
        }

        private static void GetOrCreateSelf()
        {
            if (FindAnyObjectByType<CCBehaviourStaticKernel>(FindObjectsInactive.Include)
                .Is<CCBehaviourStaticKernel>(out CCBehaviourStaticKernel found)
                )
            {
                instance = found;
                return;
            }

            var go = new GameObject("___StaticCore", typeof(CCBehaviourStaticKernel));
            instance = go.GetComponent<CCBehaviourStaticKernel>();
        }

        private CCBehaviourStatic GetInstanceOf(Type type)
        {
            Guard.IsTrue(!type.IsAbstract && !type.IsInterface,nameof(type), "Type is abstract.");
            var value = (CCBehaviourStatic?)FindAnyObjectByType(type);

            if (value == null)
                value = (CCBehaviourStatic)gameObject.AddComponent(type);

            if (instances.ContainsKey(type))
                instances[type] = value;
            else
                instances.Add(value.GetType(), value);

            return value;
        }
    }
}

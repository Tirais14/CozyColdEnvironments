using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Attributes;
using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
#pragma warning disable S2696
namespace CCEnvs.Unity
{
    [DefaultExecutionOrder(-10)]
    public sealed class CCBehaviourStaticKernel : CCBehaviour
    {
        private static WeakLazy<CCBehaviourStaticKernel> m_Self = new(GetOrCreateSelf);
        private static CCBehaviourStaticKernel self => m_Self.Value;

        private readonly Dictionary<Type, CCBehaviourStatic> instances = new();

        private readonly Lazy<GameObject> instancesGameObject = new(static () =>
        {
            var go = new GameObject($"___{nameof(CCBehaviourStatic)}Instances");

            DontDestroyOnLoad(go);

            return go;
        });

        protected override void Awake()
        {
            Validate();
            Setup();
            CreateInstantClasses();

            base.Awake();
        }

        public static CCBehaviourStatic GetOrCreateInstance(Type type)
        {
            Guard.IsTrue(!type.IsAbstract && !type.IsInterface, nameof(type), "Type is abstract.");

            if (!GetInstance(type).TryGetValue(out var instance))
                instance = CreateInstance(type);

            return instance;
        }

        public static T GetOrCreateInstance<T>()
            where T : CCBehaviourStatic
        {
            return (T)GetOrCreateInstance(typeof(T));
        }

        public static Maybe<CCBehaviourStatic> GetInstance(Type type)
        {
            Guard.IsTrue(!type.IsAbstract && !type.IsInterface, nameof(type), "Type is abstract.");

            if (!self.instances.TryGetValue(type, out var instance)
                ||
                (!ReferenceEquals(instance, null) && instance == null && instance!.DestroyedFrameIdx != UCC.CurrentFrame))
            {
                instance = (CCBehaviourStatic?)FindAnyObjectByType(type, FindObjectsInactive.Include);
            }

            return instance;
        }

        public static Maybe<T> GetInstance<T>()
            where T : CCBehaviourStatic
        {
            return GetInstance(typeof(T)).Cast<T>().RightTarget;
        }

        private void Validate()
        {
            if (FindObjectsByType<CCBehaviourStaticKernel>(
                    FindObjectsInactive.Include, FindObjectsSortMode.None).Any(x => x != this)
                )
            {
                throw new CCException($"Cannot create more than one {nameof(CCBehaviourStaticKernel)}.");
            }
        }

        private void Setup()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneUnloaded += unloadedScene =>
            {
                var toRemoveInstanceTypes =
                from instanceInfo in instances.ToArrayPooledReadOnly()
                where instanceInfo.Value == null || instanceInfo.Value.gameObject.scene == unloadedScene
                select instanceInfo.Key;

                foreach (var instanceType in toRemoveInstanceTypes)
                    instances.Remove(instanceType);
            };
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
                 .ForEachAndMaterialize(type => GetInstance(type))
                 .Length;

            CCDebug.Instance.PrintLog($"Instant created class count = {instantCreatedClassCount}.",
                             new DebugContext(this).Additive());
        }

        private static CCBehaviourStaticKernel GetOrCreateSelf()
        {
            if (FindAnyObjectByType<CCBehaviourStaticKernel>(FindObjectsInactive.Include).Let(out var found))
            {
                return found;
            }

            var go = new GameObject($"___{nameof(CCBehaviourStaticKernel)}");

            var instance = go.AddComponent<CCBehaviourStaticKernel>();

            return instance;
        }

        private static CCBehaviourStatic CreateInstance(Type type)
        {
            return (CCBehaviourStatic)self.instancesGameObject.Value.AddComponent(type);
        }
    }
}

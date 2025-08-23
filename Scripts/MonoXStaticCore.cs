using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
            if (FindAnyObjectByType<MonoXStaticCore>() != null)
                throw new LogicException($"Cannot create more than one {nameof(MonoXStaticCore)}.");

            instance = this;

            base.OnAwake();

            Type[] staticMonos = GetStaticMonos();
            CreateStaticMono(staticMonos);
            AddMonosToCollection();

            OnInstantiated += OnMonoXInstantiated;
        }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectNotFoundException"></exception>
        /// <exception cref="MissingReferenceException"></exception>
        public static MonoXStatic GetInstance(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (!instance.instances.TryGetValue(type, out MonoXStatic? result))
                throw new ObjectNotFoundException(type);
            if (result == null)
                throw new MissingReferenceException();

            return result;
        }

        private static Type[] GetStaticMonos()
        {
            return (from x in AppDomain.CurrentDomain.GetAssemblies()
                    select x.GetTypes() into types
                    from t in types
                    where t.IsType<MonoXStatic>()
                    select t)
                    .ToArray();
        }

        private void OnMonoXInstantiated(MonoX mono)
        {
            if (mono.IsNot<MonoXStatic>(out var result))
                return;

            instances.TryAdd(result.GetType(), result);
        }

        private void CreateStaticMono(Type[] staticMonos)
        {
            HashSet<Type> existingComponents = 
                FindObjectsByType<MonoXStatic>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Select(x => x.GetType())
                .Distinct()
                .ToHashSet();

            foreach (Type t in staticMonos)
            {
                if (!existingComponents.Contains(t))
                    gameObject.AddComponent(t);
            }
        }

        private void AddMonosToCollection()
        {
            instances.AddRange(FindObjectsByType<MonoXStatic>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Select(x => new KeyValuePair<Type, MonoXStatic>(x.GetType(), x)));
        }
    }
}

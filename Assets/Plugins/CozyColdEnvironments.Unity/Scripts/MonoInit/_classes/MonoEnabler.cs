using CCEnvs.UnityX.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.MonoInit
{
    [DefaultExecutionOrder(-10000)]
    public sealed class MonoEnabler : CCBehaviour
    {
        private readonly struct TypeDependencies
        {
            public readonly Type[] BeforeTypes;
            public readonly Type[] AfterTypes;

            public TypeDependencies(Type[] before, Type[] after)
            {
                BeforeTypes = before;
                AfterTypes = after;
            }
        }

        private readonly struct ComponentNode
        {
            public readonly Behaviour Component;
            public readonly Type Type;
            public readonly TypeDependencies Dependencies;

            public ComponentNode(Behaviour component, Type type, TypeDependencies dependencies)
            {
                Component = component;
                Type = type;
                Dependencies = dependencies;
            }
        }

        private static readonly Dictionary<Type, TypeDependencies> _typeDependenciesCache = new();

        // Кэш для IsAssignableFrom проверок
        private static readonly Dictionary<(Type source, Type target), bool> _assignableCache = new();

        private static TypeDependencies GetDependencies(Type type)
        {
            if (_typeDependenciesCache.TryGetValue(type, out var cached))
                return cached;

            var beforeAttrs = type.GetCustomAttributes(typeof(EnableBeforeAttribute), true);
            var afterAttrs = type.GetCustomAttributes(typeof(EnableAfterAttribute), true);

            Type[] beforeTypes;
            Type[] afterTypes;

            beforeTypes = new Type[beforeAttrs.Length];
            for (int i = 0; i < beforeAttrs.Length; i++)
                beforeTypes[i] = ((EnableBeforeAttribute)beforeAttrs[i]).Type;

            afterTypes = new Type[afterAttrs.Length];
            for (int i = 0; i < afterAttrs.Length; i++)
                afterTypes[i] = ((EnableAfterAttribute)afterAttrs[i]).Type;

            var result = new TypeDependencies(beforeTypes, afterTypes);
            _typeDependenciesCache[type] = result;
            return result;
        }

        private static bool IsAssignableToCached(Type sourceType, Type targetType)
        {
            var key = (sourceType, targetType);
            if (_assignableCache.TryGetValue(key, out var cached))
                return cached;

            var result = targetType.IsAssignableFrom(sourceType);
            _assignableCache[key] = result;
            return result;
        }

        protected override void Awake()
        {
            base.Awake();

            var components = GetComponents<Behaviour>();
            int count = components.Length;

            if (count <= 1)
                return;

            var infos = new ComponentNode[count];
            int activeCount = 0;

            // Собираем все компоненты
            for (int i = 0; i < count; i++)
            {
                var cmp = components[i];
                if (ReferenceEquals(cmp, this))
                {
                    infos[i] = default;
                    continue;
                }

                var type = cmp.GetType();
                var deps = GetDependencies(type);
                infos[i] = new ComponentNode(cmp, type, deps);
                activeCount++;
            }

            if (activeCount == 0)
                return;

            // Предварительно находим все индексы для каждого типа из атрибутов
            // Это оптимизация: вместо O(n²) проверок IsAssignableFrom делаем O(n*m)
            var typeToAssignableIndices = new Dictionary<Type, List<int>>();

            for (int i = 0; i < count; i++)
            {
                if (infos[i].Component == null) continue;

                var deps = infos[i].Dependencies;

                // Собираем все уникальные типы из атрибутов
                foreach (var beforeType in deps.BeforeTypes)
                {
                    if (!typeToAssignableIndices.ContainsKey(beforeType))
                        typeToAssignableIndices[beforeType] = new List<int>();
                }

                foreach (var afterType in deps.AfterTypes)
                {
                    if (!typeToAssignableIndices.ContainsKey(afterType))
                        typeToAssignableIndices[afterType] = new List<int>();
                }
            }

            // Для каждого типа из атрибутов находим все компоненты, assignable от него
            foreach (var kvp in typeToAssignableIndices)
            {
                var targetType = kvp.Key;
                var indices = kvp.Value;

                for (int i = 0; i < count; i++)
                {
                    if (infos[i].Component == null) continue;

                    if (IsAssignableToCached(infos[i].Type, targetType))
                    {
                        indices.Add(i);
                    }
                }
            }

            // Строим граф зависимостей
            var inDegree = new int[count];
            var adjacency = new HashSet<int>[count];
            for (int i = 0; i < count; i++)
                adjacency[i] = new HashSet<int>();

            for (int i = 0; i < count; i++)
            {
                if (infos[i].Component == null)
                    continue;

                var deps = infos[i].Dependencies;

                // EnableBefore(X): этот компонент должен быть ДО всех компонентов, assignable от X
                foreach (var beforeType in deps.BeforeTypes)
                {
                    if (!typeToAssignableIndices.TryGetValue(beforeType, out var targets))
                        continue;

                    foreach (var j in targets)
                    {
                        if (j != i && adjacency[i].Add(j))
                        {
                            inDegree[j]++;
                        }
                    }
                }

                // EnableAfter(X): этот компонент должен быть ПОСЛЕ всех компонентов, assignable от X
                foreach (var afterType in deps.AfterTypes)
                {
                    if (!typeToAssignableIndices.TryGetValue(afterType, out var sources))
                        continue;

                    foreach (var j in sources)
                    {
                        if (j != i && adjacency[j].Add(i))
                        {
                            inDegree[i]++;
                        }
                    }
                }
            }

            // Алгоритм Кана — топологическая сортировка
            var queue = new Queue<int>(activeCount);
            for (int i = 0; i < count; i++)
            {
                if (infos[i].Component != null && inDegree[i] == 0)
                    queue.Enqueue(i);
            }

            var result = new List<Behaviour>(activeCount);
            while (queue.Count > 0)
            {
                int i = queue.Dequeue();
                result.Add(infos[i].Component);

                foreach (var neighbor in adjacency[i])
                {
                    inDegree[neighbor]--;
                    if (inDegree[neighbor] == 0)
                        queue.Enqueue(neighbor);
                }
            }

            // Детекция циклов
            if (result.Count != activeCount)
            {
                var cycleTypes = new StringBuilder();
                for (int i = 0; i < count; i++)
                {
                    if (infos[i].Component != null && inDegree[i] > 0)
                    {
                        if (cycleTypes.Length > 0) cycleTypes.Append(", ");
                        cycleTypes.Append(infos[i].Type.Name);
                    }
                }

                throw new InvalidOperationException(
                    $"[MonoEnabler] Circular dependency detected on '{gameObject.name}'. " +
                    $"Components involved in cycle: {cycleTypes}. " +
                    $"Review [EnableBefore] and [EnableAfter] attributes.");
            }

            // Включаем компоненты в топологически корректном порядке
            for (int i = 0; i < result.Count; i++)
            {
                result[i].enabled = true;
            }
        }
    }
}
#nullable enable
using C5;
using CCEnvs.Attributes;
using CCEnvs.Collections.Unsafe;
using CCEnvs.Diagnostics;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CCEnvs
{
    public static class Prioritized
    {
        public static int ResolvePriority<T>(T value, bool throwIfNotFound)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            if (value is IPrioritized<int> iface)
                return iface.Priority;

            if ((typeof(T) == typeof(Type) ? value.To<Type>() : value.GetType()).GetCustomAttribute<PriorityAttribute>() is not PriorityAttribute attribute)
            {
                if (throwIfNotFound)
                    throw new CCException("Cannot resolve priority.");
                else
                    return default;
            }    

            return attribute.Priority;
        }
        public static int ResolvePriority<T>(T value)
        {
            return ResolvePriority(value, throwIfNotFound: true);
        }

        public static PrioritizedValue<T>[] Range<T>(
            Func<T, int> priorityGetter,
            params T[] values)
        {
            CC.Guard.IsNotNull(priorityGetter, nameof(priorityGetter));
            CC.Guard.IsNotNull(values, nameof(values));

            if (values.IsEmpty())
                return Array.Empty<PrioritizedValue<T>>();

            var list = new List<PrioritizedValue<T>>(values.Length);

            foreach (var item in values)
                list.Add(new PrioritizedValue<T>(item, priorityGetter(item)));

            return list.GetInternalArrayUnsafe();
        }

#if C5
        public static IntervalHeap<PrioritizedValue<T>> Heap<T>(
            Func<T, int> priorityGetter,
            params T[] values)
        {
            CC.Guard.IsNotNull(priorityGetter, nameof(priorityGetter));
            CC.Guard.IsNotNull(values, nameof(values));

            var heap = new IntervalHeap<PrioritizedValue<T>>(values.Length);

            foreach (var item in Range(priorityGetter, values))
                heap.Add(item);

            return heap;
        }
#endif //C5
    }
}

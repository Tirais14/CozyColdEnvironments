#nullable enable
using CCEnvs.Attributes;
using Humanizer;
using System;
using System.Reflection;

namespace CCEnvs
{
    public static class PrioritizedHelper
    {
        public static int ResolvePriorityByAttribute<T>(T value, bool throwIfNotFound = false)
        {
            CC.Guard.IsNotNull(value, nameof(value));

            if (typeof(T) == typeof(Type)
                ||
                value.GetType().GetCustomAttribute<PriorityAttribute>() is not PriorityAttribute attribute)
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException($"Type: {typeof(T)} doesn't contain {nameof(PriorityAttribute).Humanize()}");

                return default;
            }

            return attribute.Priority;
        }
    }
}

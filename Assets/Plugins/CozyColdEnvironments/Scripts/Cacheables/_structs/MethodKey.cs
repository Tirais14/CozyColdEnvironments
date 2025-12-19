using CCEnvs.Reflection;
using System;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Cacheables
{
    public readonly struct MethodKey : IEquatable<MethodKey>
    {
        public readonly Type ReflectedType { get; }
        public readonly Type[] ParameterTypes { get; }
        public readonly ParameterModifier ParameterModifiers { get; }

        public MethodKey(Type reflectedType,
                               Type[] parameterTypes,
                               ParameterModifier parameterModifiers)
        {
            ReflectedType = reflectedType;
            ParameterTypes = parameterTypes;
            ParameterModifiers = parameterModifiers;
        }

        public static MethodKey Create(MethodBase method)
        {
            CC.Guard.IsNotNull(method, nameof(method));

            ParameterInfo[] parameters = method.GetParameters();

            Type[] signature = parameters.Select(x => x.ParameterType).ToArray();
            ParameterModifier parameterModifiers = parameters.GetParameterModifiers();

            return new MethodKey(method.ReflectedType,
                                       signature,
                                       parameterModifiers);
        }

        public bool Equals(MethodKey other)
        {
            return ReflectedType == other.ReflectedType && ParameterTypes.SequenceEqual(other.ParameterTypes);
        }

        public override bool Equals(object? obj)
        {
            return obj is MethodKey typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(ReflectedType);

            int count = ParameterTypes.Length;
            for (int i = 0; i < count; i++)
                hash.Add(ParameterTypes[i]);

            return hash.ToHashCode();
        }
    }
}

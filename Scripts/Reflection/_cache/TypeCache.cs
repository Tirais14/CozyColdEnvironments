using CCEnvs.Cacheables;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class TypeCache
    {
        public static Dictionary<Type, object?> DefaultValuesCache { get; } = new(0);
        public static Dictionary<MethodKey, ConstructorInfo> Constructors { get; } = new(0);
        public static Dictionary<MethodKey, MethodInfo> Methods { get; } = new(0);
        public static Dictionary<FieldKey, FieldInfo> Fields { get; } = new(0);
        public static Dictionary<FieldKey, PropertyInfo> Properties { get; } = new(0);

        public static bool TryCacheDefaultValue(Type type, object? value)
        {
            return DefaultValuesCache.TryAdd(type, value);
        }

        public static bool TryCacheMember(MemberInfo member)
        {
            return member switch
            {
                ConstructorInfo ctor => Constructors.TryAdd(MethodKey.Create(ctor), ctor),
                MethodInfo method => Methods.TryAdd(MethodKey.Create(method), method),
                FieldInfo field => Fields.TryAdd(FieldKey.Create(field), field),
                PropertyInfo prop => Properties.TryAdd(FieldKey.Create(prop), prop),
                _ => throw new NotImplementedException(member.GetTypeName()),
            };
        }

        public static bool UncacheMember(MemberInfo member)
        {
            return member switch
            {
                ConstructorInfo ctor => Constructors.Remove(MethodKey.Create(ctor)),
                MethodInfo method => Methods.Remove(MethodKey.Create(method)),
                FieldInfo field => Fields.Remove(FieldKey.Create(field)),
                PropertyInfo prop => Properties.Remove(FieldKey.Create(prop)),
                _ => throw new NotImplementedException(member.GetTypeName()),
            };
        }

        public static void Clear()
        {
            DefaultValuesCache.Clear();
            Constructors.Clear();
            Methods.Clear();
            Fields.Clear();
            Properties.Clear();

            TrimExcess();
        }

        public static void TrimExcess()
        {
            DefaultValuesCache.TrimExcess();
            Constructors.TrimExcess();
            Methods.TrimExcess();
            Fields.TrimExcess();
            Properties.TrimExcess();
        }
    }
}

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using CCEnvs.Attributes;
using CCEnvs.Attributes.Serialization;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Serialization
{
    public static class TypeSerializationHelper
    {
        public static ImmutableDictionary<Type, TypeSerializationDescriptor> TypeDescriptors { get; private set; } = null!;

        public static ImmutableDictionary<TypeSerializationDescriptor, Type> DescriptedTypes { get; private set; } = null!;

        [OnInstallExecutable]
        private static void Install(MemberInfo[] domainMembers)
        {
            Guard.IsNotNull(domainMembers, nameof(domainMembers));

            var descriptedTypeInfos =
                (from type in domainMembers.AsParallel().OfType<Type>()
                 select (type, attr: type.GetCustomAttribute<SerializationDescriptorAttribute>(inherit: false)) into typeInfo
                 where typeInfo.attr is not null
                 select (typeInfo.type, desc: typeInfo.attr.ToDescriptor())
                )
                .ToArray();

            var typeDescriptorsBuilder = ImmutableDictionary.CreateBuilder<Type, TypeSerializationDescriptor>();
            var descriptedTypesBuilder = ImmutableDictionary.CreateBuilder<TypeSerializationDescriptor, Type>();

            (Type type, TypeSerializationDescriptor desc) descriptedType;

            for (int i = 0; i < descriptedTypeInfos.Length; i++)
            {
                descriptedType = descriptedTypeInfos[i];

                if (descriptedType.type.IsGenericType)
                {
                    typeof(TypeSerializationHelper).PrintError($"Generic types (type: {descriptedType.type}) is not supported");
                    continue;
                }

                typeDescriptorsBuilder.Add(descriptedType.type, descriptedType.desc);
                descriptedTypesBuilder.Add(descriptedType.desc, descriptedType.type);
            }

            TypeDescriptors = typeDescriptorsBuilder.ToImmutable();
            DescriptedTypes = descriptedTypesBuilder.ToImmutable();
        }

        public static bool TryGetTypeSerializationDescriptor(
            Type type,
            out TypeSerializationDescriptor result
            )
        {
            Guard.IsNotNull(type, nameof(type));

            if (type.IsGenericType)
            {
                result = default;
                return false;
            }

            return TypeDescriptors.TryGetValue(type, out result);
        }
    }
}

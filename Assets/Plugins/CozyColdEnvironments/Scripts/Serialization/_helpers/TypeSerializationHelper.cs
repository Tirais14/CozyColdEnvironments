using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CCEnvs.Attributes.Serialization;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Serialization
{
    public static class TypeSerializationHelper
    {
        public static IReadOnlyDictionary<Type, TypeSerializationDescriptorAttribute> TypeDescriptors { get; private set; } = null!;

        public static void Install(MemberInfo[] domainMembers)
        {
            Guard.IsNotNull(domainMembers, nameof(domainMembers));

            TypeDescriptors =
                (from type in domainMembers.AsParallel().OfType<Type>()
                 select (type, attr: type.GetCustomAttribute<TypeSerializationDescriptorAttribute>(inherit: false)) into typeInfo
                 where typeInfo.attr is not null
                 select typeInfo
                 )
                 .ToDictionary(typeInfo => typeInfo.type, typeInfo => typeInfo.attr);
        }
    }
}

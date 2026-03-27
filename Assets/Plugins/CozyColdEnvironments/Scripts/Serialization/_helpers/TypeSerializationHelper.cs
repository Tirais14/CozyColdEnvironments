using C5;
using CCEnvs.Attributes;
using CCEnvs.Attributes.Serialization;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Serialization
{
    public static class TypeSerializationHelper
    {
        private readonly static Dictionary<Type, TypeSerializationDescriptor> typeDescriptors = new();
        private readonly static Dictionary<TypeSerializationDescriptor, Type> descriptedTypes = new();

        private readonly static object typeDescriptorsGate = new();
        private readonly static object descriptedTypesGate = new();

        public static IReadOnlyDictionary<Type, TypeSerializationDescriptor> TypeDescriptors => typeDescriptors;

        public static IReadOnlyDictionary<TypeSerializationDescriptor, Type> DescriptedTypes => descriptedTypes;

        [OnInstallExecutable]
        private static void Install(MemberInfo[] domainMembers)
        {
            Guard.IsNotNull(domainMembers, nameof(domainMembers));

            typeDescriptors.Clear();
            descriptedTypes.Clear();

            (from type in domainMembers.AsParallel().OfType<Type>()
             select (type, attr: type.GetCustomAttribute<SerializationDescriptorAttribute>(inherit: false)) into typeInfo
             where typeInfo.attr is not null
             select (typeInfo.type, desc: typeInfo.attr.ToDescriptor())
            )
            .ForAll(descTypeInfo =>
            {
                lock (typeDescriptorsGate)
                    typeDescriptors.Add(descTypeInfo.type, descTypeInfo.desc);

                lock (descriptedTypesGate)
                    descriptedTypes.Add(descTypeInfo.desc, descTypeInfo.type);
            });

            AddBaseTypes();

            //var typeDescriptorsBuilder = ImmutableDictionary.CreateBuilder<Type, TypeSerializationDescriptor>();
            //var descriptedTypesBuilder = ImmutableDictionary.CreateBuilder<TypeSerializationDescriptor, Type>();

            //(Type type, TypeSerializationDescriptor desc) descriptedType;

            //for (int i = 0; i < descriptedTypeInfos.Length; i++)
            //{
            //    descriptedType = descriptedTypeInfos[i];

            //    if (descriptedType.type.IsGenericType)
            //    {
            //        typeof(TypeSerializationHelper).PrintError($"Generic types (type: {descriptedType.type}) is not supported");
            //        continue;
            //    }

            //    typeDescriptorsBuilder.Add(descriptedType.type, descriptedType.desc);
            //    descriptedTypesBuilder.Add(descriptedType.desc, descriptedType.type);
            //}

            //typeDescriptors = typeDescriptorsBuilder.ToImmutable();
            //descriptedType = descriptedTypesBuilder.ToImmutable();
        }

        private static void AddBaseTypes()
        {
            //Set<byte>(new TypeSerializationDescriptor("byte", "8cf9c587-bc39-48e4-a8a8-2096d57e1733"));
            //Set<short>(new TypeSerializationDescriptor("short", "72109447-909c-48d7-a6b2-b646a4ccc7f9"));
            //Set<ushort>(new TypeSerializationDescriptor("ushort", "9950b1cd-d1db-4d89-a1f5-9c055a222542"));
            //Set<int>(new TypeSerializationDescriptor("int", "5737e011-f4eb-4e4a-b798-79a5e5daea69"));
            //Set<uint>(new TypeSerializationDescriptor("uint", "a105ddfe-8fe4-4fed-af10-4156ba99d347"));
            //Set<long>(new TypeSerializationDescriptor("long", "439d0100-0e89-436b-aa48-db64fa4709ec"));
            //Set<ulong>(new TypeSerializationDescriptor("ulong", "9a1b952b-a582-49be-86ce-58b0a8c23118"));

//#if UNITY_2017_1_OR_NEWER
//            Set<UnityEngine.Vector2>(new TypeSerializationDescriptor("Vector2", "e09a0c05-4128-46a5-a9c2-045306598ea8"));
//            Set<UnityEngine.Vector2Int>(new TypeSerializationDescriptor("Vector2Int", "136a0f2f-25fa-4c08-b093-4c51850172f2"));
//            Set<UnityEngine.Vector3>(new TypeSerializationDescriptor("Vector3", "0cb1c150-6c44-4040-a036-87da41ad769a"));
//            Set<UnityEngine.Vector3Int>(new TypeSerializationDescriptor("Vector3Int", "8cc80cb4-790e-4ff1-b431-45fa28ee1fc1"));
//            Set<UnityEngine.Vector4>(new TypeSerializationDescriptor("Vector4", "8c5d1051-baa2-4f62-a47e-77a96a4f4ac5"));
//            Set<UnityEngine.Quaternion>(new TypeSerializationDescriptor("Quaternion", "95737e23-1c46-4a08-89b0-d5ac7ca6d923"));
//#endif
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

        public static void Set(Type type, TypeSerializationDescriptor desc)
        {
            Guard.IsNotNull(type);
            Guard.IsNotDefault(desc);

            lock (typeDescriptorsGate)
                typeDescriptors.Add(type, desc);

            lock (descriptedTypesGate)
                descriptedTypes.Add(desc, type);
        }

        public static void Set<T>(TypeSerializationDescriptor desc)
        {
            Set(typeof(T), desc);
        }

        public static void SetMany(
            params (Type Type, TypeSerializationDescriptor Desc)[] items
            )
        {
            foreach (var item in items)
                Set(item.Type, item.Desc);
        }

        public static bool Remove(TypeSerializationDescriptor desc)
        {
            lock (descriptedTypesGate)
                if (descriptedTypes.Remove(desc, out var type))
                    lock (typeDescriptorsGate)
                        return typeDescriptors.Remove(type);

            return false;
        }
    }
}

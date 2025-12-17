using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonConverterHelper
    {
        public static IList<JsonPropertyInfoCustom> ResolveJsonPropertyInfos(
            Type instanceType, 
            JsonSerializerOptions options)
        {
            Guard.IsNotNull(instanceType);
            Guard.IsNotNull(options);

            var results = new List<JsonPropertyInfoCustom>();

            string jsonPropName;
            Func<object, object?>? getter;
            Action<object, object?>? setter;
            JsonPropertyInfoCustom propInfo;
            foreach (var member in ResolveSerializableMembers(instanceType, options))
            {
                jsonPropName = ResolveJsonPropertyName(member, options);
                (getter, setter) = ResolveGetterAndSetter(member);

                propInfo = new JsonPropertyInfoCustom
                {
                    Name = jsonPropName,
                    Get = getter,
                    Set = setter
                };

                results.Add(propInfo);
            }

            return results;
        }

        /// <exception cref="ArgumentException"></exception>
        public static string ResolveJsonPropertyName(MemberInfo member, JsonSerializerOptions options)
        {
            Guard.IsNotNull(member);
            Guard.IsNotNull(options);

            var propNameAttribute = (JsonPropertyNameAttribute?)member.GetCustomAttributes().FirstOrDefault(x => x.GetType() == typeof(JsonPropertyNameAttribute));
            if (propNameAttribute is null)
            {
                if (options.PropertyNamingPolicy is not null)
                    return options.PropertyNamingPolicy.ConvertName(member.Name);
                else
                    return member.Name;
            }
            else
                return propNameAttribute.Name;
        }

        public static bool IsSerializableMember(MemberInfo member, JsonSerializerOptions options)
        {
            Guard.IsNotNull(member);
            Guard.IsNotNull(options);

            if (member.IsDefined<JsonIgnoreAttribute>())
                return false;

            bool hasJsonInclude = member.IsDefined<JsonIncludeAttribute>();
            bool ignoreReadOnlyFields = options.IgnoreReadOnlyFields;
            bool ignoreReadOnlyProps = options.IgnoreReadOnlyProperties;

            if (member is PropertyInfo prop)
            {
                if (ignoreReadOnlyProps && (prop.SetMethod is null || !prop.SetMethod.IsPublic))
                    return false;

                if (!hasJsonInclude && !prop.GetMethod.IsPublic)
                    return false;

                return true;
            }  
            else if (member is FieldInfo field)
            {
                if (ignoreReadOnlyProps && field.IsInitOnly)
                    return false;

                if (!hasJsonInclude && !field.IsPublic)
                    return false;

                return true;
            }

            return false;
        }

        public static IList<MemberInfo> ResolveSerializableMembers(
            Type type,
            JsonSerializerOptions options)
        {
            Guard.IsNotNull(type);
            Guard.IsNotNull(options);

            var results = new List<MemberInfo>();

            foreach (var member in type.GetMembers(BindingFlagsDefault.InstanceAll)
                .Where(x => x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property)
                .OrderBy(m => m.GetCustomAttribute<JsonPropertyOrderAttribute>()
                    .Maybe()
                    .Map(m => m.Order)
                    .GetValue(0)))
            {
                if (!IsSerializableMember(member, options))
                    continue;

                results.Add(member);
            }

            return results;
        }

        public static Maybe<MemberInfo> MatchMember(
            IEnumerable<MemberInfo> members,
            string jsonPropName,
            JsonSerializerOptions options)
        {
            Guard.IsNotNull(members);

            if (members.IsEmpty())
                return null;

            Guard.IsNotNullOrWhiteSpace(jsonPropName);
            Guard.IsNotNull(options);

            bool ignoreCase = options.PropertyNameCaseInsensitive;
            string memberJsonPropName;
            foreach (var member in members)
            {
                memberJsonPropName = ResolveJsonPropertyName(member, options);

                if (memberJsonPropName.EqualsOrdinal(jsonPropName, ignoreCase))
                    return member;
            }

            return null;
        }

        public static void Populate(object instance, JsonDocument doc, JsonSerializerOptions options)
        {
            Guard.IsNotNull(instance);
            Guard.IsNotNull(doc);
            Guard.IsNotNull(options);

            Type instType = instance.GetType();
            var members = ResolveSerializableMembers(instType, options);
            object? deserializedProp = null;
            Action<object, object?>? propSetter = null;
            foreach (var jsonProp in doc.RootElement.EnumerateObject())
            {
                if (jsonProp.Name == "$type")
                    continue;

                if (!MatchMember(members, jsonProp.Name, options).TryGetValue(out MemberInfo? member))
                    throw new InvalidOperationException($"Serializable member of json property '{jsonProp}' not found in type '{instType}'");

                if (member is PropertyInfo prop)
                {
                    deserializedProp = jsonProp.Value.Deserialize(prop.PropertyType);
                    propSetter = (inst, value) => prop.GetSetMethod().Invoke(inst, Range.From(value));
                }
                else if (member is FieldInfo field)
                {
                    deserializedProp = jsonProp.Value.Deserialize(field.FieldType);
                    propSetter = (inst, value) => field.SetValue(inst, value);
                }

                Guard.IsNotNull(deserializedProp);
                Guard.IsNotNull(propSetter);

                propSetter(instance, deserializedProp);
            }
        }

        private static (Func<object, object?>? getter, Action<object, object?>? setter) ResolveGetterAndSetter(MemberInfo member)
        {
            Guard.IsNotNull(member);

            Func<object, object?>? getter = null;
            Action<object, object?>? setter = null;
            if (member is PropertyInfo prop)
            {
                if (prop.CanRead)
                    getter = (Func<object, object?>?)prop.GetMethod.CreateDelegate(typeof(Func<object, object?>));

                if (prop.CanWrite && prop.SetMethod.IsPublic)
                    setter = (Action<object, object?>)prop.SetMethod.CreateDelegate(typeof(Action<object, object?>));

                return (getter, setter);
            }
            else if (member is FieldInfo field)
            {
                getter = (inst) => field.GetValue(inst);

                if (!field.IsInitOnly)
                    setter = (inst, value) => field.SetValue(inst, value); 

                return (getter, setter);
            }

            return CC.Throw.InvalidOperation(member.GetType(), nameof(member))
                .To<(Func<object, object?>? getter, Action<object, object?>? setter)>();
        }
    }
}

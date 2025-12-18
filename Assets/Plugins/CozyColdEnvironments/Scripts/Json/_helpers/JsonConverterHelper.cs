using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonConverterHelper
    {
        public static IList<JsonPropertyInfo> ResolveJsonPropertyInfos(
            Type instanceType, 
            JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(instanceType);
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            var results = new List<JsonPropertyInfo>();

            string jsonPropName;
            Func<object, object?>? getter;
            Action<object, object?>? setter;
            JsonPropertyInfo propInfo;
            foreach (var member in ResolveSerializableMembers(instanceType, settings))
            {
                jsonPropName = ResolveJsonPropertyName(member, settings);
                (getter, setter) = ResolveGetterAndSetter(member);

                propInfo = new JsonPropertyInfo
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
        public static string ResolveJsonPropertyName(
            MemberInfo member,
            JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(member);
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            var jProperty = member.GetCustomAttribute<JsonPropertyAttribute>();

            var namingStrategy = settings.ContractResolver?.Reflect()
                .NonPublic()
                .TypeFilter<NamingStrategy>()
                .Property()
                .Lax()
                .Map(x => x.GetValue(settings.ContractResolver))
                .Cast<NamingStrategy>()
                .RightTarget;

            if (jProperty is null || jProperty.PropertyName.IsNullOrWhiteSpace())
            {
                if (namingStrategy is not null)
                    return namingStrategy.GetPropertyName(member.Name, hasSpecifiedName: false);

                return member.Name;
            }
            else
                return jProperty.PropertyName;
        }

        public static bool IsSerializableMember(
            MemberInfo member,
            JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(member);
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            if (member.IsDefined<JsonIgnoreAttribute>())
                return false;

            bool isDefinedJProperty = member.IsDefined<JsonPropertyAttribute>();
            if (member is PropertyInfo prop)
            {
                if (!isDefinedJProperty && (prop.SetMethod is null || !prop.SetMethod.IsPublic))
                    return false;

                if (!isDefinedJProperty && !prop.GetMethod.IsPublic)
                    return false;

                return true;
            }  
            else if (member is FieldInfo field)
            {
                if (!isDefinedJProperty && field.IsInitOnly)
                    return false;

                if (!isDefinedJProperty && !field.IsPublic)
                    return false;

                return true;
            }

            return false;
        }

        public static IList<MemberInfo> ResolveSerializableMembers(
            Type type,
            JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(type);
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            var results = new List<MemberInfo>();

            foreach (var member in from member in type.GetMembers(BindingFlagsDefault.InstanceAll)
                where member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property
                select (member, attribute: member.GetCustomAttribute<JsonPropertyAttribute>()) into pair
                orderby pair.attribute?.Order ?? 0
                select pair.member)
            {
                if (!IsSerializableMember(member, settings))
                    continue;

                results.Add(member);
            }

            return results;
        }

        public static Maybe<MemberInfo> MatchMember(
            IEnumerable<MemberInfo> members,
            string jsonPropName,
            JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(members);

            if (members.IsEmpty())
                return null;

            Guard.IsNotNullOrWhiteSpace(jsonPropName);
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            string memberJsonPropName;
            foreach (var member in members)
            {
                memberJsonPropName = ResolveJsonPropertyName(member, settings);

                if (memberJsonPropName.EqualsOrdinal(jsonPropName, ignoreCase: false))
                    return member;
            }

            return null;
        }

        public static void Populate(
            object instance,
            JObject jObject,
            JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(instance);
            Guard.IsNotNull(jObject);
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            Type instType = instance.GetType();
            var members = ResolveSerializableMembers(instType, settings);
            object? deserializedProp = null;
            Action<object, object?>? propSetter = null;
            foreach (var jProp in jObject.Properties())
            {
                if (jProp.Name == "$type")
                    continue;

                if (!MatchMember(members, jProp.Name, settings).TryGetValue(out MemberInfo? member))
                    throw new InvalidOperationException($"Serializable member of json property not found in type '{instType}'");

                if (member is PropertyInfo prop)
                {
                    deserializedProp = JsonConvert.DeserializeObject(jProp.ToString(), prop.PropertyType, settings);
                    propSetter = (inst, value) => prop.GetSetMethod().Invoke(inst, Range.From(value));
                }
                else if (member is FieldInfo field)
                {
                    deserializedProp = JsonConvert.DeserializeObject(jProp.ToString(), field.FieldType, settings);
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
                    getter = (inst) => prop.GetValue(inst);

                if (prop.CanWrite && prop.SetMethod.IsPublic)
                    setter = (inst, value) => prop.SetValue(inst, value);

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

using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Json
{
    public static class JsonConverterHelper
    {
        public static IList<JsonPropertyInfo> ResolveJsonPropertyInfos(
            Type type, 
            JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(type);
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            return type.Reflect()
                .IncludeNonPublic()
                .IncludeInstance()
                .ValuedMembers()
                .Select(member => new JsonPropertyInfo(member))
                .ToArray();
        }

        [DebuggerStepThrough]
        public static string ResolveJsonPropertyName(
            MemberInfo member,
            JsonSerializerSettings? settings = null)
        {
            return ResolveJsonPropertyName((object)member, settings);
        }

        [DebuggerStepThrough]
        public static string ResolveJsonPropertyName(
            ParameterInfo member,
            JsonSerializerSettings? settings = null)
        {
            return ResolveJsonPropertyName((object)member, settings);
        }

        /// <exception cref="ArgumentException"></exception>
        private static string ResolveJsonPropertyName(
            object memberUntyped,
            JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(memberUntyped);

            settings ??= JsonSerializerSettingsProvider.GetDefault();
            JsonPropertyAttribute jPropAttribute;
            string name;

            if (memberUntyped is MemberInfo member)
            {
                jPropAttribute = member.GetCustomAttribute<JsonPropertyAttribute>();
                name = member.Name;
            }
            else if (memberUntyped is ParameterInfo param)
            {
                jPropAttribute = param.GetCustomAttribute<JsonPropertyAttribute>();
                name = param.Name;
            }
            else
                throw new ArgumentException(nameof(memberUntyped));

            var namingStrategy = settings.ContractResolver?.Reflect()
                .Cache()
                .IncludeNonPublic()
                .IncludeInstance()
                .WithTypeFilter<NamingStrategy>()
                .GetPropertyValue<NamingStrategy>()
                .Raw;

            if (jPropAttribute is null || jPropAttribute.PropertyName.IsNullOrWhiteSpace())
            {
                if (namingStrategy is not null)
                    return namingStrategy.GetPropertyName(name, hasSpecifiedName: false);

                return name;
            }
            else
                return jPropAttribute.PropertyName;
        }

        //public static bool IsSerializableMember(
        //    MemberInfo member,
        //    JsonSerializerSettings? settings = null)
        //{
        //    Guard.IsNotNull(member);
        //    settings ??= JsonSerializerSettingsProvider.GetDefault();

        //    if (member.IsDefined<JsonIgnoreAttribute>())
        //        return false;

        //    bool isDefinedJProperty = member.IsDefined<JsonPropertyAttribute>();
        //    if (member is PropertyInfo prop)
        //    {
        //        if (!isDefinedJProperty && prop.GetAccessors().All(x => !x.IsPublic))
        //            return false;

        //        return true;
        //    }  
        //    else if (member is FieldInfo field)
        //    {
        //        if (!isDefinedJProperty && field.IsInitOnly)
        //            return false;

        //        if (!isDefinedJProperty && !field.IsPublic)
        //            return false;

        //        return true;
        //    }

        //    return false;
        //}

        //public static IList<MemberInfo> ResolveSerializableMembers(
        //    Type type,
        //    JsonSerializerSettings? settings = null)
        //{
        //    Guard.IsNotNull(type);
        //    settings ??= JsonSerializerSettingsProvider.GetDefault();

        //    var results = new List<MemberInfo>();

        //    foreach (var member in from member in type.GetMembers(BindingFlagsDefault.InstanceAll)
        //        where member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property
        //        select (member, attribute: member.GetCustomAttribute<JsonPropertyAttribute>()) into pair
        //        orderby pair.attribute?.Order ?? 0
        //        select pair.member)
        //    {
        //        if (!IsSerializableMember(member, settings))
        //            continue;

        //        results.Add(member);
        //    }

        //    return results;
        //}

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
            ICollection<JProperty> jProps,
            JsonSerializer jSerializer)
        {
            Guard.IsNotNull(instance);
            Guard.IsNotNull(jProps);

            if (jProps.IsEmpty())
                return;

            Type instType = instance.GetType();
            var jPropInfos = ResolveJsonPropertyInfos(instType, jSerializer.GetSerializerSettings());

            foreach (var jProp in jProps)
            {
                foreach (var jPropInfo in jPropInfos)
                {
                    if (!jPropInfo.Match(jProp))
                        continue;

                    object? deserializedProp = jProp.Value.ToObject(jPropInfo.UnderlyingType, jSerializer);

                    if (jPropInfo.Set is not null && jPropInfo.ValuedMember is not null)
                        jPropInfo.Set(instance, deserializedProp);
                }
            }
        }

        public static (ConstructorInfo ctor, IList<JProperty> jProps)? ResolveConstructor(
            Type type,
            ICollection<JProperty> jProps,
            JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(type, nameof(type));
            CC.Guard.IsNotNull(jProps, nameof(jProps));
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            var ctors = type.Reflect()
                .Cache()
                .Constructors()
                .Select(ctor => (ctor, prms: ctor.GetParameters()))
                .Where(pair => pair.prms.IsEmpty() || pair.ctor.IsDefined<JsonConstructorAttribute>())
                .OrderByDescending(pair => pair.prms.Length)
                .ToArray();

            if (jProps.IsNotNullOrEmpty())
            {
                var orderedJProps = new List<JProperty>(jProps.Count);
                var usedJProps = new HashSet<JProperty>(jProps.Count);
                foreach (var (ctor, prms) in ctors)
                {
                    int requiredParamCount = prms.Count(param => !param.HasDefaultValue);
                    if (requiredParamCount > jProps.Count)
                        continue;

                    bool isFound = true;
                    foreach (var param in prms)
                    {
                        string paramName = ResolveJsonPropertyName(param);

                        var matchingJProp = jProps.FirstOrDefault(jProp =>
                            jProp.Name.Equals(paramName, StringComparison.OrdinalIgnoreCase)
                            );

                        if (matchingJProp is not null && !usedJProps.Contains(matchingJProp))
                        {
                            orderedJProps.Add(matchingJProp);
                            usedJProps.Add(matchingJProp);
                        }
                        else if (param.HasDefaultValue)
                            orderedJProps.Add(new JProperty(paramName));
                        else
                        {
                            isFound = false;
                            break;
                        }
                    }

                    if (isFound && orderedJProps.Count >= requiredParamCount)
                        return (ctor, orderedJProps);

                    orderedJProps.Clear();
                    usedJProps.Clear();
                }
            }
            else if (ctors.FirstOrDefault(pair => pair.prms.IsEmpty())
                .Maybe()
                .Map(pair => pair.ctor)
                .TryGetValue(out var ctor))
            {
                return (ctor, Array.Empty<JProperty>());
            }

            return null;
        }

        public static object CreateInstance(
            Type type, 
            ICollection<JProperty> jProps,
            JsonSerializer jSerializer)
        {
            Guard.IsNotNull(type, nameof(type));

            JsonSerializerSettings settings = jSerializer.GetSerializerSettings();
            var resolved = ResolveConstructor(type, jProps, settings) ?? throw new JsonException($"Cannot resolve constructor of type \"{type}\"");
            var jPropInfos = ResolveJsonPropertyInfos(type, settings);

            var deserializedJProps = new List<object?>(jProps.Count);
            foreach (var jProp in resolved.jProps)
            {
                if (!jPropInfos.FirstOrDefault(jPropInfo => jPropInfo.Match(jProp)).Let(out var matchingJPropInfo))
                    continue;

                object? deserializedJProp = jProp.Value.ToObject(matchingJPropInfo.UnderlyingType, jSerializer);
                deserializedJProps.Add(deserializedJProp);
            }

            var instance = resolved.ctor.Invoke(deserializedJProps.ToArray());

            if (resolved.ctor.GetParameters().IsEmpty() && jProps.IsNotEmpty())
                Populate(instance, jProps, jSerializer);

            return instance;
        }
    }
}

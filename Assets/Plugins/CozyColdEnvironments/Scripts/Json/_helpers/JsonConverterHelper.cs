using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
                .Select(member => new JsonPropertyInfo(member, settings))
                .ToArray();
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

        public static (ConstructorInfo? ctor, IList<JProperty> jProps) ResolveConstructor(
            Type type,
            ICollection<JProperty> jProps,
            JsonSerializerSettings? settings = null
            )
        {
            Guard.IsNotNull(type, nameof(type));
            CC.Guard.IsNotNull(jProps, nameof(jProps));
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            using var ctors = ListPool<(ConstructorInfo ctor, ParameterInfo[] prms)>.Shared.Get();

            foreach (var ctor in type.GetConstructors(BindingFlagsDefault.All))
            {
                if (ctor.IsDefined<JsonConstructorAttribute>())
                {
                    ctors.Value.Clear();
                    ctors.Value.Add((ctor, ctor.GetParameters()));
                    break;
                }

                ctors.Value.Add((ctor, ctor.GetParameters()));
            }

            if (ctors.Value.Count > 1)
            {
                ctors.Value.Sort(
                    static (left, right) =>
                    {
                        var comaprer = Comparer<int>.Default;

                        return -comaprer.Compare(left.prms.Length, right.prms.Length);
                    });
            }

            if (jProps.IsNotNullOrEmpty())
            {
                using var orderedJProps = ListPool<JProperty>.Shared.Get();

                if (orderedJProps.Value.Capacity < jProps.Count)
                    orderedJProps.Value.Capacity = jProps.Count;

                using var usedJProps = HashSetPool<JProperty>.Shared.Get();

                if (usedJProps.Value.Count < jProps.Count)
                    usedJProps.Value.EnsureCapacity(jProps.Count);

                foreach (var (ctor, prms) in ctors.Value)
                {
                    int requiredParamCount = prms.Count(param => !param.HasDefaultValue);
                    if (requiredParamCount > jProps.Count)
                        continue;

                    bool isFound = true;
                    foreach (var param in prms)
                    {
                        string paramName = ResolveJsonPropertyName(param, settings);

                        var matchingJProp = jProps.FirstOrDefault(jProp =>
                            jProp.Name.Equals(paramName, StringComparison.OrdinalIgnoreCase)
                            );

                        if (matchingJProp is not null && !usedJProps.Value.Contains(matchingJProp))
                        {
                            orderedJProps.Value.Add(matchingJProp);
                            usedJProps.Value.Add(matchingJProp);
                        }
                        else if (param.HasDefaultValue)
                            orderedJProps.Value.Add(new JProperty(paramName));
                        else
                        {
                            isFound = false;
                            break;
                        }
                    }

                    if (isFound && orderedJProps.Value.Count >= requiredParamCount)
                        return (ctor, orderedJProps.Value);

                    orderedJProps.Value.Clear();
                    usedJProps.Value.Clear();
                }
            }
            else if (!type.IsValueType
                &&
                ctors.Value.FirstOrDefault(pair => pair.prms.IsEmpty())
                .Maybe()
                .Map(pair => pair.ctor)
                .TryGetValue(out var ctor))
            {
                return (ctor, Array.Empty<JProperty>());
            }

            return (null, Array.Empty<JProperty>());
        }

        public static object CreateInstance(
            Type type,
            ICollection<JProperty> jProps,
            JsonSerializer jSerializer)
        {
            Guard.IsNotNull(type, nameof(type));

            JsonSerializerSettings settings = jSerializer.GetSerializerSettings();

            var ctorInfo = ResolveConstructor(type, jProps, settings);

            var jPropInfos = ResolveJsonPropertyInfos(type, settings);

            var deserializedJProps = ListPool<object?>.Shared.Get();

            if (deserializedJProps.Value.Capacity < jProps.Count)
                deserializedJProps.Value.Capacity = jProps.Count;

            foreach (JProperty jProp in ctorInfo.jProps)
            {
                if (!jPropInfos.FirstOrDefault(jPropInfo =>
                    {
                        return jPropInfo.Match(jProp);
                    })
                    .Let(out var matchingJPropInfo))
                {
                    continue;
                }

                object? deserializedJProp = jProp.Value.ToObject(matchingJPropInfo.UnderlyingType, jSerializer);
                deserializedJProps.Value.Add(deserializedJProp);
            }

            object instance;

            if (ctorInfo.ctor is null)
            {
                if (!type.IsValueType)
                    throw new JsonException($"Cannot resolve constructor of type \"{type}\"");

                instance = Activator.CreateInstance(type);
            }
            else
                instance = ctorInfo.ctor.Invoke(deserializedJProps.Value.ToArray());

            Populate(instance, jProps, jSerializer);

            return instance;
        }

        private readonly static Lazy<Type> jsonInternalReaderType = new(
            static () =>
            {
                return Type.GetType($"Newtonsoft.Json.Serialization.JsonSerializerInternalReader, Newtonsoft.Json", throwOnError: true);
            });

        private static MethodInfo? createObjectMethod;

        private static PropertyInfo? parametrizedCreatorProp;

        public static object CreateNewObject(
            Type type,
            JsonReader reader,
            JsonSerializer serializer
            )
        {
            Guard.IsNotNull(type, nameof(type));
            Guard.IsNotNull(reader, nameof(reader));
            Guard.IsNotNull(serializer, nameof(serializer));

            var contract = serializer.ContractResolver.ResolveContract(type);

            var intReader = CreateJsonSerializerInternalReader(serializer);

            var createObjectMethod = GetCreateObjectMethod();

            var prms = new object?[] //CreateObject
            {
                reader,
                contract,
                null,
                null,
                null,
                false,
            };
             
            //Magic offset :>
            reader.Read();
            reader.Read();

            var instance = createObjectMethod.Invoke(intReader, prms);

            return instance;
        }

        private static object CreateJsonSerializerInternalReader(JsonSerializer serializer)
        {
            var jSerializerIntReaderType = jsonInternalReaderType.Value;

            return Activator.CreateInstance(jSerializerIntReaderType, serializer);
        }

        private static MethodInfo GetCreateObjectMethod()
        {
            if (createObjectMethod is not null)
                return createObjectMethod;

            var methods = jsonInternalReaderType.Value.GetMethods(BindingFlagsDefault.InstancePublic | BindingFlags.DeclaredOnly);

            createObjectMethod = methods.FirstOrDefault(method => method.Name == "CreateNewObject")
                ??
                throw new InvalidOperationException("Cannot find method: CreateNewObject");

            return createObjectMethod;
        }

        private static ObjectConstructor<object>? GetParametrizedCreator(JsonObjectContract contract)
        {
            if (parametrizedCreatorProp is not null)
            {
                return parametrizedCreatorProp.GetValue(contract)
                    .To<ObjectConstructor<object>?>();
            }

            parametrizedCreatorProp = TypeofCache<JsonObjectContract>.Type.GetProperty("ParameterizedCreator", BindingFlagsDefault.InstanceNonPublic);

            return parametrizedCreatorProp.GetValue(contract)
                    .To<ObjectConstructor<object>?>();
        }
    }
}

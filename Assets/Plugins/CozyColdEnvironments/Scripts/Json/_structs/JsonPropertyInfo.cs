using System;
using System.Reflection;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

#nullable enable
namespace CCEnvs.Json
{
    public sealed class JsonPropertyInfo
    {
        public object Source { get; init; }
        public ValuedMemberInfo? ValuedMember => Source as ValuedMemberInfo;
        public string Name { get; init; }
        public Func<object, object?>? Get { get; init; }
        public Action<object, object?>? Set { get; init; }
        public Type UnderlyingType { get; init; }
        public Required Required { get; init; }
        public bool ShouldSerialize { get; init; }
        public int Order { get; init; }

        public JsonPropertyInfo(ValuedMemberInfo memberInfo, JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(memberInfo, nameof(memberInfo));
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            Source = memberInfo;

            var jPropAttribute = memberInfo.Member.GetCustomAttribute<JsonPropertyAttribute>();

            Name = JsonConverterHelper.ResolveJsonPropertyName(memberInfo.Member, settings);
            UnderlyingType = memberInfo.UnderlyingType;
            Get = memberInfo.ValueGetter;
            Set = memberInfo.ValueSetter;
            ShouldSerialize = IsSerializable(memberInfo, jPropAttribute);

            if (jPropAttribute is not null)
            {
                Required = jPropAttribute.Required;
                Order = jPropAttribute.Order;
            }
        }

        public JsonPropertyInfo(ParameterInfo param, JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(param, nameof(param));

            settings ??= JsonSerializerSettingsProvider.GetDefault();
            Source = param.Member;
            Name = JsonConverterHelper.ResolveJsonPropertyName(param, settings);
        }

        private static bool IsSerializable(
            ValuedMemberInfo memberInfo,
            JsonPropertyAttribute? jPropAttribute)
        {
            if (jPropAttribute is not null)
                return true;

            if (memberInfo.Member.IsDefined<JsonIgnoreAttribute>())
                return false;

            return memberInfo.MemberType == MemberTypes.Property && memberInfo.CanRead;
        }

        public bool Match(JsonProperty jProp)
        {
            Guard.IsNotNull(jProp, nameof(jProp));

            if (jProp.PropertyName != Name)
                return false;

            if (jProp.PropertyType != UnderlyingType)
                return false;

            return true;
        }

        public bool Match(JProperty jProp)
        {
            Guard.IsNotNull(jProp, nameof(jProp));

            if (jProp.Name != Name)
                return false;

            return true;
        }

        public bool Match(JsonPropertyInfo jPropInfo)
        {
            Guard.IsNotNull(jPropInfo, nameof(jPropInfo));

            if (jPropInfo.Source != Source)
                return false;

            if (jPropInfo.Name != Name)
                return false;

            if (jPropInfo.UnderlyingType != UnderlyingType)
                return false;

            return true;
        }

        public bool Match(object member)
        {
            if (member is PropertyInfo prop)
                return Match(new JsonPropertyInfo(prop));

            if (member is ValuedMemberInfo valuedMember)
                return Match(new JsonPropertyInfo(valuedMember));

            if (member is FieldInfo field)
                return Match(new JsonPropertyInfo(field));

            if (member is ParameterInfo param)
                return Match(new JsonPropertyInfo(param));

            return false;
        }

        public override string ToString()
        {
            return $"Name: {Name}; should serialize: {ShouldSerialize}; underlying type: {UnderlyingType}";
        }
    }
}

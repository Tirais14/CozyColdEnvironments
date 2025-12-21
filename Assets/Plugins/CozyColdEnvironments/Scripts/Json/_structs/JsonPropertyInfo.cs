using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Json
{
    public readonly struct JsonPropertyInfo
    {
        public readonly ValuedMemberInfo Member { get; }
        public readonly string Name { get; }
        public readonly Func<object, object?>? Get { get; } 
        public readonly Action<object, object?>? Set { get; }
        public readonly Type UnderlyingType { get; }
        public readonly Required Required { get; }
        public readonly bool ShouldSerialize { get; }
        public readonly int Order { get; }

        public JsonPropertyInfo(ValuedMemberInfo valuedMember, JsonSerializerSettings? settings = null)
        {
            Guard.IsNotNull(valuedMember, nameof(valuedMember));
            settings ??= JsonSerializerSettingsProvider.GetDefault();

            Member = valuedMember;
            Get = valuedMember.ValueGetter;
            Set = valuedMember.ValueSetter;
            UnderlyingType = valuedMember.UnderlyingType;

            var jPropAttribute = valuedMember.GetCustomAttribute<JsonPropertyAttribute>();

            Name = jPropAttribute.PropertyName ?? valuedMember.Name;
            Order = jPropAttribute?.Order ?? 0;

            if (jPropAttribute is not null)
            {
                Required = jPropAttribute.Required;
                ShouldSerialize = true;
            }
            else
            {
                ShouldSerialize = IsSerializable(valuedMember);
            }
        }

        private static bool IsSerializable(ValuedMemberInfo member)
        {
            return member.MemberType == MemberTypes.Property && member.CanRead;
        }
    }
}

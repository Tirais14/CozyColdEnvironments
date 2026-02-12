using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public struct Reflected
    {
        public Type? Type { get; set; }

        public BindingFlags Bindings { get; set; }

        public StringMatchSettings StringMatchSettings { get; set; }

        public string? NameFilter { get; set; }

        public static Reflected Create()
        {
            return new Reflected
            {
                Bindings = BindingFlags.Default,
                StringMatchSettings = StringMatchSettings.Default,
            };
        }

        public readonly bool IsNameMatch(string name)
        {
            if (NameFilter.IsNullOrWhiteSpace())
                return true;

            return NameFilter.Match(name, StringMatchSettings);
        }

        public readonly MemberInfo[] GetMembers(MemberTypes memberType)
        {
            Guard.IsNotNull(Type, nameof(Type));

            return memberType switch
            {
                MemberTypes.All => Type.GetMembers(Bindings),
                MemberTypes.Constructor => Type.GetConstructors(Bindings),
                MemberTypes.Custom => throw new NotImplementedException(),
                MemberTypes.Event => Type.GetEvents(Bindings),
                MemberTypes.Field => Type.GetFields(Bindings),
                MemberTypes.Method => Type.GetMethods(Bindings),
                MemberTypes.NestedType => Type.GetNestedTypes(Bindings),
                MemberTypes.Property => Type.GetProperties(Bindings),
                MemberTypes.TypeInfo => throw new NotImplementedException(),
                _ => throw new InvalidOperationException(memberType.ToString())
            };
        }
    }
}

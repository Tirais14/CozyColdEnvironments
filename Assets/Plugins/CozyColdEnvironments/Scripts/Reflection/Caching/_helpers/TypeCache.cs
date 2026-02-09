using CCEnvs.Caching;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection.Caching
{
    public static class TypeCache
    {
        private readonly static Cache<MemberKey, MemberInfo> members = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private readonly static Cache<ParameterKey, ParameterInfo> parameters = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private readonly static Cache<MethodKey, MethodBase> methods = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        public static bool TryGetMember(
            MemberKey key, 
            [NotNullWhen(true)] out MemberInfo? member
            )
        {
            CC.Guard.IsNotDefault(key, nameof(key));

            return members.TryGet(key, out member);
        }

        public static bool TryGetParameter(
            ParameterKey key,
            [NotNullWhen(true)] out ParameterInfo? param
            )
        {
            CC.Guard.IsNotDefault(key, nameof(key));

            return parameters.TryGet(key, out param);
        }

        public static bool TryGetMethod(
            MethodKey key,
            [NotNullWhen(true)] out MethodInfo? method
            )
        {
            CC.Guard.IsNotDefault(key, nameof(key));

            if (!methods.TryGet(key, out var tMethod))
            {
                method = null;
                return false;
            }

            method = (MethodInfo)tMethod;
            return true;
        }

        public static bool TryGetConstructor(
            MethodKey key,
            [NotNullWhen(true)] out ConstructorInfo? constructor
            )
        {
            CC.Guard.IsNotDefault(key, nameof(key));

            key = key.WithMemberPart(key.MemberPart.WithName(".ctor"));

            if (!methods.TryGet(key, out var tConstructor))
            {
                constructor = null;
                return false;
            }

            constructor = (ConstructorInfo)tConstructor;
            return false;
        }

        public static void AddMember(
            MemberInfo member, 
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(member, nameof(member));

            if (members.TryAdd(member, member, out var entry))
                entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? 20.Minutes();
        }


        public static void AddParameter(
            ParameterInfo param,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(param, nameof(param));

            if (parameters.TryAdd(param, param, out var entry))
                entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? 20.Minutes();
        }

        public static void AddMethod(
            MethodInfo method,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(method, nameof(method));

            if (methods.TryAdd(method, method, out var entry))
                entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? 20.Minutes();
        }

        public static void AddMethod(
            ConstructorInfo ctor,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(ctor, nameof(ctor));

            if (methods.TryAdd(ctor, ctor, out var entry))
                entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? 20.Minutes();
        }
    }
}

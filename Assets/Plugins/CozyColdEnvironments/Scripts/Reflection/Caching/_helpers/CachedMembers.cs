using CCEnvs.Caching;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection.Caching
{
    public static class CachedMembers
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

        private readonly static Cache<MethodBase, Delegate> methodDelegates = new();

        private readonly static Cache<MembersKey, MemberInfo[]> typeMembers = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        public static bool TryGetMember(
            MemberKey key, 
            [NotNullWhen(true)] out MemberInfo? member
            )
        {
            return members.TryGet(key, out member);
        }

        public static bool TryGetParameter(
            ParameterKey key,
            [NotNullWhen(true)] out ParameterInfo? param
            )
        {
            return parameters.TryGet(key, out param);
        }

        public static bool TryGetMethod(
            MethodKey key,
            [NotNullWhen(true)] out MethodInfo? method
            )
        {
            if (!methods.TryGet(key, out var tMethod))
            {
                method = null;
                return false;
            }

            method = (MethodInfo)tMethod;
            return true;
        }

        public static bool TryGetMethodDelegate(
            MethodBase method,
            [NotNullWhen(true)] out Delegate? dlg
            )
        {
            return methodDelegates.TryGet(method, out dlg);
        }

        public static bool TryGetConstructor(
            MethodKey key,
            [NotNullWhen(true)] out ConstructorInfo? constructor
            )
        {
            key = key.WithMemberPart(key.MemberPart.WithName(".ctor"));

            if (!methods.TryGet(key, out var tConstructor))
            {
                constructor = null;
                return false;
            }

            constructor = (ConstructorInfo)tConstructor;
            return false;
        }

        public static bool TryGetTypeMembers(
            MembersKey key,
            [NotNullWhen(true)] out MemberInfo[]? members
            )
        {
            return typeMembers.TryGet(key, out members);
        }

        public static bool TryAddMember(
            MemberInfo member, 
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(member, nameof(member));

            if (!members.TryAdd(member, member, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? 20.Minutes();
            return true;
        }


        public static bool TRyAddParameter(
            ParameterInfo param,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(param, nameof(param));

            if (!parameters.TryAdd(param, param, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? 20.Minutes();
            return true;
        }

        public static bool TryAddMethod(
            MethodInfo method,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(method, nameof(method));

            if (!methods.TryAdd(method, method, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? 20.Minutes();
            return true;
        }

        public static bool TryAddMethod(
            ConstructorInfo ctor,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(ctor, nameof(ctor));

            if (!methods.TryAdd(ctor, ctor, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? 20.Minutes();
            return true;
        }

        public static bool TryAddMethodDelegate(
            MethodBase method,
            Delegate dlg,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(method, nameof(method));

            if (!methodDelegates.TryAdd(method, dlg, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? 20.Minutes();
            return true;
        }

        public static bool TryAddTypeMembers(
            MembersKey key, 
            IEnumerable<MemberInfo> members,
            TimeSpan? expirationTimeRelativeToNow = null)
        {
            CC.Guard.IsNotNull(members, nameof(members));

            if (typeMembers.ContainsKey(key))
                return false;

            var entry = typeMembers.CreateEntry(key);

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? 20.Minutes();

            entry.SetValue(members.ToArray());

            return true;
        }
    }
}

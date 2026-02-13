using CCEnvs.Caching;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.Diagnostics.CodeAnalysis;
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

        private readonly static Cache<FieldKey, FieldInfo> fields = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private readonly static Cache<PropertyKey, PropertyInfo> props = new()
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

        public static TimeSpan DefaultExpirationTimeRelativeToNow { get; set; } = 20.Minutes();

        #region Getters

        public static bool TryGetMember(
            MemberKey key, 
            [NotNullWhen(true)] out MemberInfo? member
            )
        {
            return members.TryGet(key, out member);
        }

        public static bool TryGetField(
            FieldKey key,
            [NotNullWhen(true)] out FieldInfo? field
            )
        {
            return fields.TryGet(key, out field);
        }

        public static bool TryGetProperty(
            PropertyKey key,
            [NotNullWhen(true)] out PropertyInfo? prop
            )
        {
            return props.TryGet(key, out prop);
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

        public static bool TryGetConstructor(
            MethodKey key,
            [NotNullWhen(true)] out ConstructorInfo? constructor
            )
        {
            key = key.WithMemberPart(key.Core.WithName(".ctor"));

            if (!methods.TryGet(key, out var tConstructor))
            {
                constructor = null;
                return false;
            }

            constructor = (ConstructorInfo)tConstructor;
            return false;
        }

        public static bool TryGetMemberUntyped(
            object key,
            MemberTypes memberType,
            [NotNullWhen(true)] out MemberInfo? member
            )
        {
            switch (memberType)
            {
                case MemberTypes.All:
                    throw new NotImplementedException();
                case MemberTypes.Constructor:
                    {
                        if (key is not MethodKey ctorKey)
                        {
                            member = null;
                            return false;
                        }

                        if (!methods.TryGet(ctorKey, out var ctor))
                        {
                            member = null;
                            return false;
                        }

                        member = ctor;
                        return true;
                    }
                case MemberTypes.Custom:
                    throw new NotImplementedException();
                case MemberTypes.Event:
                    throw new NotImplementedException();
                case MemberTypes.Field:
                    {
                        if (key is not FieldKey fieldKey)
                        {
                            member = null;
                            return false;
                        }

                        if (!fields.TryGet(fieldKey, out var field))
                        {
                            member = null;
                            return false;
                        }

                        member = field;
                        return true;
                    }
                case MemberTypes.Method:
                    {
                        if (key is not MethodKey methodKey)
                        {
                            member = null;
                            return false;
                        }

                        if (!methods.TryGet(methodKey, out var method))
                        {
                            member = null;
                            return false;
                        }

                        member = method;
                        return true;
                    }
                case MemberTypes.NestedType:
                    throw new NotImplementedException();
                case MemberTypes.Property:
                    {
                        if (key is not PropertyKey propKey)
                        {
                            member = null;
                            return false;
                        }

                        if (!props.TryGet(propKey, out var prop))
                        {
                            member = null;
                            return false;
                        }

                        member = prop;
                        return true;
                    }
                case MemberTypes.TypeInfo:
                    throw new NotImplementedException();
                default:
                    throw new InvalidOperationException(memberType.ToString());
            }
        }

        #endregion Getters

        #region AddMethods

        public static bool TryAddMember(
            MemberInfo member, 
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(member, nameof(member));

            if (!members.TryAdd(member, member, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
            return true;
        }

        public static bool TryAddField(
            FieldInfo field,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(field, nameof(field));

            if (!fields.TryAdd(field, field, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
            return true;
        }

        public static bool TryAddProperty(
            PropertyInfo prop,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(prop, nameof(prop));

            if (!props.TryAdd(prop, prop, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
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

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
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

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
            return true;
        }

        public static bool TryAddConstructor(
            ConstructorInfo ctor,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(ctor, nameof(ctor));

            if (!methods.TryAdd(ctor, ctor, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
            return true;
        }

        public static bool TryAddMemberUntyped(
            MemberInfo member,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(member, nameof(member));

            expirationTimeRelativeToNow ??= 20.Minutes();

            return member switch
            {
                MethodInfo method => TryAddMethod(method, expirationTimeRelativeToNow),
                ConstructorInfo ctor => TryAddConstructor(ctor, expirationTimeRelativeToNow),
                FieldInfo field => TryAddField(field, expirationTimeRelativeToNow),
                PropertyInfo prop => TryAddProperty(prop, expirationTimeRelativeToNow),
                MemberInfo => TryAddMember(member, expirationTimeRelativeToNow),
                _ => throw new InvalidOperationException(member.GetType().Name),
            };
        }

        #endregion AddMethods
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CCEnvs.Caching;
using CommunityToolkit.Diagnostics;
using Humanizer;

#nullable enable
namespace CCEnvs.Reflection.Caching
{
    public static class CachedMembers
    {
        private readonly static Cache<MemberKey, MemberInfo> members = new()
        {
            ExpirationScanFrequency = DefaultExpirationScanFrequency,
            SizeLimit = DefaultSizeLimit
        };

        private readonly static Cache<FieldKey, FieldInfo> fields = new()
        {
            ExpirationScanFrequency = DefaultExpirationScanFrequency,
            SizeLimit = DefaultSizeLimit
        };

        private readonly static Cache<PropertyKey, PropertyInfo> props = new()
        {
            ExpirationScanFrequency = DefaultExpirationScanFrequency,
            SizeLimit = DefaultSizeLimit
        };

        private readonly static Cache<ParameterKey, ParameterInfo> parameters = new()
        {
            ExpirationScanFrequency = DefaultExpirationScanFrequency,
            SizeLimit = DefaultSizeLimit
        };

        private readonly static Cache<MethodKey, MethodBase> methods = new()
        {
            ExpirationScanFrequency = DefaultExpirationScanFrequency,
            SizeLimit = DefaultSizeLimit
        };

        private static TimeSpan perMemberScanFrequency;

        private static int? perMemberSizeLimit;

        public static TimeSpan DefaultExpirationTimeRelativeToNow { get; set; } = 20.Minutes();

        public static TimeSpan DefaultExpirationScanFrequency { get; } = 1.Minutes();

        public static TimeSpan ExpirationScanFrequency {
            get => perMemberScanFrequency;
            set
            {
                perMemberScanFrequency = value;

                members.ExpirationScanFrequency = perMemberScanFrequency;
                fields.ExpirationScanFrequency = perMemberScanFrequency;
                props.ExpirationScanFrequency = perMemberScanFrequency;
                parameters.ExpirationScanFrequency = perMemberScanFrequency;
                methods.ExpirationScanFrequency = perMemberScanFrequency;
            }
        }

        public static int? DefaultSizeLimit { get; } = 163840;

        public static int? SizeLimit {
            get => perMemberSizeLimit;
            set
            {
                perMemberSizeLimit = value;

                members.SizeLimit = perMemberSizeLimit;
                fields.SizeLimit = perMemberSizeLimit;
                props.SizeLimit = perMemberSizeLimit;
                parameters.SizeLimit = perMemberSizeLimit;
                methods.SizeLimit = perMemberSizeLimit;
            }
        }

        #region Getters

        public static bool TryGetMember(
            MemberKey key,
            [NotNullWhen(true)] out MemberInfo? member
            )
        {
            return members.TryGetValue(key, out member);
        }

        public static bool TryGetField(
            FieldKey key,
            [NotNullWhen(true)] out FieldInfo? field
            )
        {
            return fields.TryGetValue(key, out field);
        }

        public static bool TryGetProperty(
            PropertyKey key,
            [NotNullWhen(true)] out PropertyInfo? prop
            )
        {
            return props.TryGetValue(key, out prop);
        }

        public static bool TryGetParameter(
            ParameterKey key,
            [NotNullWhen(true)] out ParameterInfo? param
            )
        {
            return parameters.TryGetValue(key, out param);
        }

        public static bool TryGetMethod(
            MethodKey key,
            [NotNullWhen(true)] out MethodInfo? method
            )
        {
            if (!methods.TryGetValue(key, out var tMethod))
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

            if (!methods.TryGetValue(key, out var tConstructor))
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

                        if (!methods.TryGetValue(ctorKey, out var ctor))
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

                        if (!fields.TryGetValue(fieldKey, out var field))
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

                        if (!methods.TryGetValue(methodKey, out var method))
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

                        if (!props.TryGetValue(propKey, out var prop))
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
            out MemberKey key,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(member, nameof(member));

            key = member;

            if (!members.TryAdd(key, member, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
            return true;
        }

        public static bool TryAddField(
            FieldInfo field,
            out FieldKey key,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(field, nameof(field));

            key = field;

            if (!fields.TryAdd(key, field, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
            return true;
        }

        public static bool TryAddProperty(
            PropertyInfo prop,
            out PropertyKey key,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(prop, nameof(prop));

            key = prop;

            if (!props.TryAdd(key, prop, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
            return true;
        }

        public static bool TryAddParameter(
            ParameterInfo param,
            out ParameterKey key,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(param, nameof(param));

            key = param;

            if (!parameters.TryAdd(key, param, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
            return true;
        }

        public static bool TryAddMethod(
            MethodInfo method,
            out MethodKey key,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(method, nameof(method));

            key = method;

            if (!methods.TryAdd(key, method, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
            return true;
        }

        public static bool TryAddConstructor(
            ConstructorInfo ctor,
            out MethodKey key,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(ctor, nameof(ctor));

            key = ctor;

            if (!methods.TryAdd(key, ctor, out var entry))
                return false;

            entry.ExpirationTimeRelativeToNow = expirationTimeRelativeToNow ?? DefaultExpirationTimeRelativeToNow;
            return true;
        }

        public static bool TryAddMemberUntyped(
            MemberInfo member,
            out object key,
            TimeSpan? expirationTimeRelativeToNow = null
            )
        {
            Guard.IsNotNull(member, nameof(member));

            expirationTimeRelativeToNow ??= 20.Minutes();

            if (member is MethodInfo method
                &&
                TryAddMethod(method, out var methodKey, expirationTimeRelativeToNow))
            {
                key = methodKey;
                return true;
            }
            else if (member is ConstructorInfo ctor
                &&
                TryAddConstructor(ctor, out var ctorKey, expirationTimeRelativeToNow))
            {
                key = ctorKey;
                return true;
            }
            else if (member is FieldInfo field
                &&
                TryAddField(field, out var fieldKey, expirationTimeRelativeToNow))
            {
                key = fieldKey;
                return true;
            }
            else if (member is PropertyInfo prop
                &&
                TryAddProperty(prop, out var propKey, expirationTimeRelativeToNow))
            {
                key = propKey;
                return true;
            }
            else if (TryAddMember(member, out var memberKey, expirationTimeRelativeToNow))
            {
                key = memberKey;
                return true;
            }

            throw new InvalidOperationException(member.GetType().FullName);
        }

        #endregion AddMethods
    }
}

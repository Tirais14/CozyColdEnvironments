using CCEnvs.Diagnostics;
using CCEnvs.Reflection.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static CCEnvs.BindingFlagsDefault;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class TypeReflectionUtils
    {
        /// <summary>
        /// Extended version
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bindings"></param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(this Type value,
            MethodBindings bindings,
            bool throwIfNotFound = false)
        {
            Validate.ArgumentNull(value, nameof(value));
            Validate.ArgumentNull(bindings, nameof(bindings));

            if (value.GetConstructor(bindings.BindingFlags,
                                    bindings.Binder,
                                    bindings.CallingConventions,
                                    bindings.Arguments.GetTypes(),
                                    bindings.ParameterModifiersArray
                                    )
                is ConstructorInfo found
                )
                return found;

            ConstructorInfo[] constructors = value.GetConstructors(
                bindings.BindingFlags);

            found = (from x in constructors
                     let parameters = x.GetCCParameters()
                     where parameters == ((CCParameters)bindings.Arguments)
                     select x).FirstOrDefault();

            if (throwIfNotFound && found is null)
                throw new ConstructorNotFoundException(
                    value,
                    bindings.BindingFlags,
                    (CCParameters)bindings.Arguments);

            return found;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static MemberInfo[] GetMembers(Type value,
            Type memberType,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            Validate.ArgumentNull(value, nameof(value));
            Validate.ArgumentNull(memberType, nameof(memberType));

            MemberInfo[] members = value.GetMembers(bindingFlags);
            List<MemberInfo> results = new();

            MemberInfo member;
            int membersCount = members.Length;
            for (int i = 0; i < membersCount; i++)
            {
                member = members[i];
                if (member.IsType(memberType))
                    results.Add(member);
            }

            return results.ToArray();
        }

        public static MemberInfo[] ForceGetMembers(this Type value,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            Validate.ArgumentNull(value, nameof(value));

            Queue<Type> baseTypes = TypeHelper.CollectBaseTypes(value);
            var loopPredicate = new LoopPredicate(() => baseTypes.Count > 0);
            var members = new List<MemberInfo>();
            bindingFlags |= BindingFlags.DeclaredOnly;
            while (loopPredicate)
                members.AddRange(baseTypes.Dequeue().GetMembers(bindingFlags));

            return members.ToArray();
        }
        public static T[] ForceGetMembers<T>(this Type value,
            BindingFlags bindingFlags = InstancePublic)

            where T : MemberInfo
        {
            Validate.ArgumentNull(value, nameof(value));

            return value.ForceGetMembers(bindingFlags).OfType<T>().ToArray();
        }

        public static FieldInfo[] ForceGetFields(this Type value,
            BindingFlags bindingFlags = InstancePublic)
        {
            Validate.ArgumentNull(value, nameof(value));

            return value.ForceGetMembers<FieldInfo>(bindingFlags);
        }

        public static FieldInfo? ForceGetField(this Type value,
            string fieldName,
            BindingFlags bindingFlags = InstancePublic)
        {
            Validate.ArgumentNull(value, nameof(value));

            return value.ForceGetFields(bindingFlags).FirstOrDefault(x => x.Name == fieldName);
        }

        public static PropertyInfo[] ForceGetProperties(this Type value,
            BindingFlags bindingFlags = InstancePublic)
        {
            Validate.ArgumentNull(value, nameof(value));

            return value.ForceGetMembers<PropertyInfo>(bindingFlags);
        }

        public static PropertyInfo? ForceGetProperty(this Type value,
            string propName,
            BindingFlags bindingFlags = InstancePublic)
        {
            Validate.ArgumentNull(value, nameof(value));

            return value.ForceGetProperties(bindingFlags).FirstOrDefault(x => x.Name == propName);
        }

        public static MethodInfo[] ForceGetMethods(this Type value,
            BindingFlags bindingFlags = InstancePublic)
        {
            Validate.ArgumentNull(value, nameof(value));

            return value.ForceGetMembers<MethodInfo>(bindingFlags);
        }

        public static MethodInfo? ForceGetMethod(this Type value,
            string methodName,
            Type[]? types = null,
            BindingFlags bindingFlags = InstancePublic)
        {
            Validate.ArgumentNull(value, nameof(value));

            types ??= Type.EmptyTypes;

            return value.ForceGetMethods(bindingFlags)
                        .FirstOrDefault(x => x.Name == methodName 
                                &&
                                x.GetParameters().Select(x => x.ParameterType)
                                    .SequenceEqual(types));
        }

        public static MethodInfo[] GetOverloadedCastOperators(this Type value)
        {
            Validate.ArgumentNull(value, nameof(value));

            return value.ForceGetMethods(StaticAll)
                        .Where(x => x.Name == "op_Implicit" || x.Name == "op_Explicit")
                        .ToArray();
        }

        public static MethodInfo GetOverloadedCastOperator(this Type value, Type castType)
        {
            Validate.ArgumentNull(value, nameof(value));
            Validate.ArgumentNull(castType, nameof(castType));

            return value.GetOverloadedCastOperators().First(x => x.ReturnType == castType);
        }

        public static ConstructorInfo[] ForceGetConstructors(this Type value,
            BindingFlags bindingFlags = InstancePublic)
        {
            return value.ForceGetMembers<ConstructorInfo>(bindingFlags);
        }

        public static ConstructorInfo? ForceGetConstructor(this Type value,
            MethodBindings bindings)
        {
            Validate.ArgumentNull(value, nameof(value));

            ConstructorInfo[] ctors = value.ForceGetConstructors(bindings.BindingFlags);

            return ctors.FirstOrDefault(x => ConstructorBindingsMatcher.IsMatch(bindings, x));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static MethodInfo? GetMethod(
            this Type value,
            MethodBindings bindings,
            bool throwIfNotFound = false)
        {
            Validate.ArgumentNull(value, nameof(value));
            Validate.ArgumentNull(bindings, nameof(bindings));

            MethodInfo? method = value.GetMethod(bindings.Name,
                                                 bindings.GenericArguments.Length,
                                                 bindings.BindingFlags,
                                                 bindings.Binder,
                                                 (Type[])bindings.Arguments,
                                                 bindings.ParameterModifiersArray);

            if (method is null)
            {
                if (throwIfNotFound)
                    throw new MethodNotFoundException(
                        value,
                        bindings.Name,
                        bindings.BindingFlags);
                else
                    return method;
            }

            if (bindings.HasGenericArguments)
                method = method.MakeGenericMethod(bindings.GenericArguments);

            return method;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static FieldInfo[] GetFields(this Type value,
            Predicate<FieldInfo> filter,
            BindingFlags bindingFlags = InstancePublic)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (filter is null)
                throw new ArgumentNullException(nameof(filter));

            FieldInfo[] fields = value.GetFields(bindingFlags);

            return fields.Where(x => filter(x)).ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static FieldInfo? GetField(this Type value,
            Predicate<FieldInfo> filter,
            BindingFlags bindingFlags = InstancePublic)
        {
            return value.GetFields(filter, bindingFlags).FirstOrDefault();
        }
        public static FieldInfo? GetField(this Type value,
            Type fieldType,
            BindingFlags bindingFlags = InstancePublic)
        {
            if (fieldType is null)
                throw new ArgumentNullException(nameof(fieldType));

            return value.GetField(x => x.FieldType.IsType(fieldType), bindingFlags);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static PropertyInfo[] GetProperties(this Type value,
            Predicate<PropertyInfo> filter,
            BindingFlags bindingFlags = InstancePublic)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (filter is null)
                throw new ArgumentNullException(nameof(filter));

            PropertyInfo[] props = value.GetProperties(bindingFlags);

            return props.Where(x => filter(x)).ToArray();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static PropertyInfo? GetProperty(this Type value,
            Predicate<PropertyInfo> filter,
            BindingFlags bindingFlags = InstancePublic)
        {
            return value.GetProperties(filter, bindingFlags).FirstOrDefault();
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static PropertyInfo? GetProperty(this Type value,
            Type propertyType,
            BindingFlags bindingFlags = InstancePublic)
        {
            if (propertyType is null)
                throw new ArgumentNullException(nameof(propertyType));

            return value.GetProperty(x => x.PropertyType.IsType(propertyType), bindingFlags);
        }
    }
}

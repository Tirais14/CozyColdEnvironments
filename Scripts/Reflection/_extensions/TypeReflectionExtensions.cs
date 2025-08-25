using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection.ObjectModel;
using static UTIRLib.BindingFlagsDefault;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class TypeReflectionExtensions
    {
        /// <summary>
        /// Extended version
        /// </summary>
        /// <param name="type"></param>
        /// <param name="constructorParameters"></param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(this Type type,
            ConstructorBindings constructorParameters,
            bool throwIfNotFound = false)
        {
            if (type.GetConstructor(constructorParameters.BindingFlags,
                                    constructorParameters.Binder,
                                    constructorParameters.CallingConventions,
                                    (Type[])constructorParameters.Arguments,
                                    constructorParameters.ParameterModifiersArray
                                    )
                is ConstructorInfo found
                                    )
                return found;

            ConstructorInfo[] constructors = type.GetConstructors(
                constructorParameters.BindingFlags);

            found = constructors.FirstOrDefault(x =>
            {
                ParameterInfo[] parameters = x.GetParameters();

                if (!constructorParameters.ParameterModifiers
                        .Equals(parameters.GetParameterModifiers())
                        )
                    return false;

                return constructorParameters.Arguments.signature == new Signature(parameters.Select(x => x.ParameterType));

                //This for filter default params, cause bugs
                //bool result = signature.IsMatch(parameters,
                //                                signature.AllowInheritance,
                //                                ignoreDefaultValues: true);


                //if (result && constructorParameters.Arguments.Length < parameters.Length)
                //{
                //    var newArgs = new object[parameters.Length];
                //    for (int i = 0; i < newArgs.Length; i++)
                //        newArgs[i] = parameters[i].DefaultValue;

                //    constructorParameters.Arguments.CopyTo(newArgs, 0);

                //    signature = new InvokableSignature(parameters.Select(x => x.ParameterType));

                //    constructorParameters.Arguments = newArgs;
                //    constructorParameters.Signature = signature;
                //}

                //return result;
            });

            if (throwIfNotFound && found is null)
                throw new MemberNotFoundException(type,
                                                  MemberType.Constructor,
                                                  constructorParameters);

            return found;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static MemberInfo[] GetMembers(Type type,
            Type memberType,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (memberType is null)
                throw new ArgumentNullException(nameof(memberType));

            MemberInfo[] members = type.GetMembers(bindingFlags);
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

        public static MemberInfo[] ForceGetMembers(this Type type,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Queue<Type> baseTypes = Collector.Collect(type, (x, loopState) =>
            {
                Type? next = x.BaseType;

                if (next is null || next == typeof(object))
                {
                    loopState.Value = LoopKeyword.Break;
                    return null;
                }

                return next!;
            });

            bindingFlags |= BindingFlags.DeclaredOnly;
            var members = new List<MemberInfo>();

            var loopPredicate = new LoopPredicate(() => baseTypes.Count > 0);
            while (loopPredicate.Invoke())
                members.AddRange(baseTypes.Dequeue().GetMembers(bindingFlags));

            return members.ToArray();
        }
        public static T[] ForceGetMembers<T>(this Type value,
            BindingFlags bindingFlags = InstancePublic)

            where T : MemberInfo
        {
            return value.ForceGetMembers(bindingFlags).OfType<T>().ToArray();
        }

        public static FieldInfo[] ForceGetFields(this Type value,
            BindingFlags bindingFlags = InstancePublic)
        {
            return value.ForceGetMembers<FieldInfo>(bindingFlags);
        }

        public static PropertyInfo[] ForceGetProperties(this Type value,
            BindingFlags bindingFlags = InstancePublic)
        {
            return value.ForceGetMembers<PropertyInfo>(bindingFlags);
        }

        public static MethodInfo[] ForceGetMethods(this Type value,
            BindingFlags bindingFlags = InstancePublic)
        {
            return value.ForceGetMembers<MethodInfo>(bindingFlags);
        }

        public static ConstructorInfo[] ForceGetConstructors(this Type value,
            BindingFlags bindingFlags = InstancePublic)
        {
            return value.ForceGetMembers<ConstructorInfo>(bindingFlags);
        }

        public static ConstructorInfo? ForceGetConstructor(this Type value,
            ConstructorBindings bindings)
        {
            ConstructorInfo[] ctors = value.ForceGetConstructors(bindings.BindingFlags);

            return ctors.FirstOrDefault(x => ConstructorBindingsMatcher.IsMatch(bindings, x));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static MethodInfo? GetMethod(
            this Type value,
            MethodBindings bindings,
            bool throwIfNotFound = false)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (bindings is null)
                throw new ArgumentNullException(nameof(bindings));

            MethodInfo? method = value.GetMethod(bindings.MethodName,
                                                 bindings.GenericArguments.Length,
                                                 bindings.BindingFlags,
                                                 bindings.Binder,
                                                 (Type[])bindings.Arguments,
                                                 bindings.ParameterModifiersArray);

            if (method is null)
            {
                if (throwIfNotFound)
                    throw new MemberNotFoundException(value, MemberType.Method, bindings);
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

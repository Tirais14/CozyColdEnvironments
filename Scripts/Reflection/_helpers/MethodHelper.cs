using CCEnvs.Diagnostics;
using CCEnvs.Reflection.ObjectModel;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class MethodHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        /// <exception cref="MemberNotFoundException"></exception>
        public static object? Invoke(TypeValuePair target,
                                     string methodName,
                                     ExplicitArguments args = default,
                                     Signature genericParams = default,
                                     ParameterModifier parameterModifier = default)
        {
            if (target.type is null)
                throw new ArgumentException(nameof(target));
            if (methodName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(methodName), methodName);

            BindingFlags bindingFlags = target.value.IsNull()
                ?
                BindingFlagsDefault.StaticAll
                :
                BindingFlagsDefault.InstanceAll;

            MethodInfo method = target.type.GetMethod(
                methodName,
                genericParams.Count,
                bindingFlags,
                binder: null,
                (Type[])args,
                new ParameterModifier[] { parameterModifier })
                ??
                throw new MemberNotFoundException(
                    target.type,
                    MemberType.Method,
                    new MethodBindings
                    {
                        MethodName = methodName,
                        BindingFlags = bindingFlags,
                        Arguments = args,
                        GenericArguments = (Type[])genericParams
                    });

            if (genericParams.IsNotEmpty() && method.IsGenericMethod)
                method = method.MakeGenericMethod((Type[])genericParams);

            return method.Invoke(target.value, (object?[])args);
        }
        public static object? Invoke(object target,
                                     string methodName,
                                     ExplicitArguments args = default,
                                     Signature genericParams = default)
        {
            return Invoke(new TypeValuePair(target), methodName, args, genericParams);
        }

        public static T? Invoke<T>(TypeValuePair target,
                                   string methodName,
                                   ExplicitArguments args = default,
                                   Signature genericParams = default)
        {
            return (T?)Invoke(target, methodName, args, genericParams);
        }
        public static T? Invoke<T>(object target,
                                   string methodName,
                                   ExplicitArguments args = default,
                                   Signature genericParams = default)
        {
            return (T?)Invoke(target, methodName, args, genericParams);
        }
    }
}

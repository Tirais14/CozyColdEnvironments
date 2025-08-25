using System;
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection.ObjectModel;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class MethodHelper
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        /// <exception cref="MemberNotFoundException"></exception>
        public static object? Invoke(TypeValuePair target,
                                     string methodName,
                                     ExplicitArguments args = default,
                                     Signature genericParams = default)
        {
            if (target.type is null)
                throw new ArgumentException(nameof(target));
            if (methodName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(methodName), methodName);

            MethodInfo method = target.type.GetMethod(methodName, BindingFlagsDefault.All)
                ?? throw new MemberNotFoundException(
                    target.type,
                    MemberType.Method,
                    new MethodBindings
                    {
                        MethodName = methodName,
                        BindingFlags = BindingFlagsDefault.All,
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

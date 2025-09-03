using CCEnvs.Diagnostics;
using CCEnvs.Reflection.Data;
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
                                     ParameterModifier parameterModifier = default,
                                     params Type[] genericParams)
        {
            if (target.Type is null)
                throw new ArgumentException(nameof(target));
            if (methodName.IsNullOrEmpty())
                throw new StringArgumentException(nameof(methodName), methodName);

            BindingFlags bindingFlags = target.Value.IsNull()
                ?
                BindingFlagsDefault.StaticAll
                :
                BindingFlagsDefault.InstanceAll;

            MethodInfo method = target.Type.GetMethod(
                methodName,
                genericParams.Length,
                bindingFlags,
                binder: null,
                (Type[])args,
                new ParameterModifier[] { parameterModifier })
                ??
                throw new MethodNotFoundException(
                    target.Type,
                    methodName,
                    bindingFlags);

            if (genericParams.IsNotEmpty() && method.IsGenericMethod)
                method = method.MakeGenericMethod(genericParams);

            return method.Invoke(target.Value, (object?[])args);
        }
        public static object? Invoke(object target,
                                     string methodName,
                                     ExplicitArguments args = default,
                                     params Type[] genericParams)
        {
            return Invoke(new TypeValuePair(target),
                          methodName,
                          args,
                          parameterModifier: default,
                          genericParams);
        }

        public static T? Invoke<T>(TypeValuePair target,
                                   string methodName,
                                   ExplicitArguments args = default,
                                   params Type[] genericParams)
        {
            return (T?)Invoke(target,
                              methodName,
                              args,
                              genericParams);
        }
        public static T? Invoke<T>(object target,
                                   string methodName,
                                   ExplicitArguments args = default,
                                   params Type[] genericParams)
        {
            return (T?)Invoke(target,
                              methodName,
                              args,
                              genericParams);
        }
    }
}

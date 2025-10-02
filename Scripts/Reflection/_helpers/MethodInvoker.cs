using CCEnvs.Reflection.Data;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class MethodInvoker
    {
        public static object? Invoke(TypeValuePair target,
                                     string methodName,
                                     ExplicitArguments args = default,
                                     params Type[] genericParams)
        {
            CC.Guard.NullArgument(target.Type,
                                  Syntax.Chain(nameof(target), nameof(target.Type)));
            CC.Guard.StringArgument(methodName, nameof(methodName));

            BindingFlags bindingFlags = BindingFlagsDefault.All;

            MethodInfo method = target.Type.GetMethod(
                methodName,
                genericParams.Length,
                bindingFlags,
                binder: null,
                args.GetTypes(),
                Range.From(args.GetParameterModifiers()))
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

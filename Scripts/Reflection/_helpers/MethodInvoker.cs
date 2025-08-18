using System;
using System.Reflection;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.Reflection
{
    public static class MethodInvoker
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="StringArgumentException"></exception>
        /// <exception cref="MemberNotFoundException"></exception>
        public static object? Invoke(Type type,
                                     MethodBindings bindings)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (bindings is null)
                throw new ArgumentNullException(nameof(bindings));

            MethodInfo method = type.GetMethod(bindings, throwIfNotFound: true)!;

            return method.Invoke(bindings.Target, bindings.Arguments);
        }

        public static T? Invoke<T>(Type value,
                                   MethodBindings bindings)
        {
            return (T?)Invoke(value, bindings);
        }
    }
}

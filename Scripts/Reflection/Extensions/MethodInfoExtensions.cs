using System;
using System.Linq;
using System.Reflection;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.Reflection
{
    public static class MethodInfoExtensions
    {
        public static InvokableSignature GetSignature(this MethodInfo method)
        {
            return new InvokableSignature(method.GetParameters().Select(x => x.ParameterType));
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static object Invoke(this MethodBase method,
                                    object target,
                                    params object[] parameters)
        {
            if (target.IsNull())
                throw new ArgumentNullException(nameof(target));

            return method.Invoke(target, parameters);
        }
    }
}
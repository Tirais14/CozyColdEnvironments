using System;
using System.Linq;
using System.Reflection;
using CCEnvs.Diagnostics;
using CCEnvs.Reflection.Data;

#nullable enable

namespace CCEnvs.Reflection
{
    public static class MethodInfoExtensions
    {
        public static Signature GetSignature(this MethodInfo method)
        {
            return new Signature(method.GetParameters().Select(x => x.ParameterType));
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
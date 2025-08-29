using System;
using System.Linq;
using System.Reflection;
using CozyColdEnvironments.Diagnostics;
using CozyColdEnvironments.Reflection.ObjectModel;

#nullable enable

namespace CozyColdEnvironments.Reflection
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
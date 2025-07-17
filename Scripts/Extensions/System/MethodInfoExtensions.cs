using System;
using System.Reflection;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.Extensions
{
    public static class MethodInfoExtensions
    {
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
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
        public static CCParameters GetCCParameters(this MethodInfo method, 
            bool ignoreOptionalParameters = false)
        {
            return new CCParameters(method.GetParameters()
                .Select(x => x.AsCCParameterInfo()).ToArray())
            {
                IgnoreOptionalInEquals = ignoreOptionalParameters
            };
        }
    }
}
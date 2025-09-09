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
        public static CCParameters GetCCParameters(this MethodBase value, 
            bool ignoreOptionalParameters = false)
        {
            return new CCParameters(value.GetParameters()
                .Select(x => x.AsCCParameterInfo()).ToArray())
            {
                IgnoreOptionalInEquals = ignoreOptionalParameters
            };
        }
    }
}
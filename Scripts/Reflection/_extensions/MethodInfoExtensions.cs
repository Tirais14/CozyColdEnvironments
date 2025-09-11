using CCEnvs.Reflection.Data;
using System.Linq;
using System.Reflection;

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
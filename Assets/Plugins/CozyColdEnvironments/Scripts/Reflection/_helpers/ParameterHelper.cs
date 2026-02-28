using CCEnvs.Pools;
using System.Collections.Generic;
using System.Reflection;

#nullable enable
namespace CCEnvs.Reflection
{
    public static class ParameterHelper
    {
        public static ParameterKey[] ToParamteterKeys(this IEnumerable<ParameterInfo> source)
        {
            CC.Guard.IsNotNullSource(source);

            using var paramKeys = ListPool<ParameterKey>.Shared.Get();

            foreach (var param in source)
                paramKeys.Value.Add(new ParameterKey(param));

            return paramKeys.Value.ToArray();
        }
    }
}

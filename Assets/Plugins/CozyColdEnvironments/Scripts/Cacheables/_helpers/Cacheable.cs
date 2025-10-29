using CCEnvs.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CCEnvs.Reflection.Data;

#nullable enable
namespace CCEnvs.Cacheables
{
    public static class Cacheable
    {
        private readonly static HashSet<Type> cacheableTypes = new(0);

        public static bool IsCacheable(Type type)
        {
            CC.Guard.IsNotNull(type, nameof(type));
            if (cacheableTypes.Contains(type))
                return true;

            bool result = false;
            if (type.IsType<ICacheable>())
                result = true;

            if (!result)
                result = type.IsDefined<CacheableAttribute>(inherit: false);

            if (result)
                cacheableTypes.Add(type);

            return result;
        }
        public static bool IsCacheable<T>(T obj)
        {
            CC.Guard.IsNotNull(obj, nameof(obj));

            if (obj is ICacheable)
                return true;

            return IsCacheable(obj.GetType());
        }

        public static bool TryGetCache(Type type,
                                       [NotNullWhen(true)] out object? result,
                                       ExplicitArguments args = default)
        {
            CC.Guard.IsNotNull(type, nameof(type));
            if (IsCacheable(type))
            {
                result = null;
                return false;
            }

            if (TryGetCacheByAccessors(type, args, out result))
                return true;

            return false;
        }

        private static bool TryGetCacheByAccessors(Type type,
                                                   ExplicitArguments args,
                                                   [NotNullWhen(true)] out object? result)
        {
            IEnumerable<MethodInfo> accessors =
                from method in type.ForceGetMethods(BindingFlagsDefault.StaticAll)
                where method.IsDefined<CacheAccessorAttribute>(inherit: true)
                where method.GetCCParameters(ignoreOptionalParameters: true) == (CCParameters)args
                select method;

            result = accessors.FirstOrDefault()?.Invoke(null, ((object?[])args));

            return result is not null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CozyColdEnvironments.Diagnostics;
using CozyColdEnvironments.Reflection;
using CozyColdEnvironments.Unity.TypeMatching;

#nullable enable
namespace CozyColdEnvironments.Tickables
{
    public static class Tickable
    {
        public static bool TryGetTickerType(ITickableBase tickable,
            [NotNullWhen(true)] out Type? result)
        {
            result = null;

            if (tickable.IsNull())
                return false;

            if (TryGetTickerTypeAttribute(tickable, out TickerTypeAttribute? attribute))
            {
                result = attribute.TickerType;
                return true;
            }

            TryGetTickerTypeByInterfaces(tickable, out result);

            return result is not null;
        }

        public static bool TryGetTickerTypeByInterfaces(ITickableBase tickable,
            [NotNullWhen(true)] out Type? result)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));

            IEnumerable<Type> intefaceTypes =
                from x in TypeHelper.CollectBaseTypes(tickable.GetType())
                select x.GetInterfaces() into types
                from t in types
                select t;

            result = (from x in intefaceTypes
                      where x.IsType<ITickable>()
                      where x.IsGenericType
                      select x.GetGenericArguments() into types
                      from t in types
                      select t).FirstOrDefault();

            return result is not null;
        }

        public static bool TryGetTickerTypeAttribute(ITickableBase? tickable,
            [NotNullWhen(true)] out TickerTypeAttribute? result)
        {
            if (tickable.IsNull())
            {
                result = null;
                return false;
            }

            return tickable.GetType().GetCustomAttribute(typeof(TickerTypeAttribute))
                .Is<TickerTypeAttribute>(out result);
        }

        public static bool HasTickerInfo(ITickableBase? tickable)
        {
            if (tickable.IsNull())
                return false;
            if (!TryGetTickerType(tickable, out _))
                return false;

            return true;
        }
    }
}

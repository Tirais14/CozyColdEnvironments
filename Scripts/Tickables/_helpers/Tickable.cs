using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Reflection;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.Tickables
{
    public static class Tickable
    {
        public static bool TryGetTickerType(ITickableBase tickable,
            [NotNullWhen(true)] out Type? result)
        {
            result = null;

            if (tickable.IsNull())
                return false;

            if (TryGetTickerTypeAttribute(tickable, out TickerTypeAttribute attribute))
            {
                result = attribute.TickerType;
                return true;
            }

            Type[] genericArguments = tickable.GetType().GetGenericArguments();

            if (genericArguments.IsEmpty())
                return false;

            result = genericArguments.FirstOrDefault(x => x.IsType<ITicker>());
            return result != null;
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
    }
}

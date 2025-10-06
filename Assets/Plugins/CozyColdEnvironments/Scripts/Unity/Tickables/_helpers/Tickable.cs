using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using ZLinq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#nullable enable
namespace CCEnvs.Unity.Tickables
{
    public static class Tickable
    {
        public const string TICKER_PROPERTY_NAME = "t_Ticker";
        public const string TICKER_SUBSCRIPTION_PROPERTY_NAME = "t_Subscription";

        public static bool HasTickerInfo<T>(T? tickable)
            where T : ITickableBase
        {
            if (tickable.IsNull() || !TryGetTickerType(tickable, out _))
                return false;

            return true;
        }

        public static Type GetTickerType<T>(T tickable)
            where T : ITickableBase
        {
            CC.Guard.NullArgument(tickable, nameof(tickable));

            if (GetTickerTypeAttribute(tickable) is TickerTypeAttribute attribute)
                return attribute.TickerType;

            return GetTickerTypeByInterfaces(tickable)
                   ??
                   throw new CCException($"Cannot resolve ticker type of tickable: {tickable.GetType().GetName()}. Specify explicitly.");
        }

        public static bool TryGetTickerType<T>(T tickable,
            [NotNullWhen(true)] out Type? result)
            where T : ITickableBase
        {
            try
            {
                result = GetTickerType(tickable);
            }
            catch (CCException)
            {
                result = null;
                return false;
            }

            return result is not null;
        }

        private static Type? GetTickerTypeByInterfaces<T>(T tickable)
            where T : ITickableBase
        {
            CC.Guard.NullArgument(tickable, nameof(tickable));

            return (from baseType in TypeHelper.CollectBaseTypes(tickable.GetType()).AsValueEnumerable()
                    select baseType.GetInterfaces() into ifaces
                    from iface in ifaces
                    where iface.IsType<ITickableBase>()
                    where iface.IsGenericType
                    select iface.GetGenericArguments() into genericArgs
                    from genericArg in genericArgs
                    select genericArg)
                    .FirstOrDefault();
        }

        private static TickerTypeAttribute? GetTickerTypeAttribute<T>(T? tickable)
            where T : ITickableBase
        {
            CC.Guard.NullArgument(tickable, nameof(tickable));

            return tickable.GetType().GetCustomAttribute<TickerTypeAttribute>();
        }
    }
}

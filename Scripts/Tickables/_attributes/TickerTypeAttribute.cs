using System;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.Tickables
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TickerTypeAttribute : Attribute
    {
        public Type TickerType { get; }

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public TickerTypeAttribute(Type tickerType)
        {
            if (tickerType is null)
                throw new ArgumentNullException(nameof(tickerType));
            if (tickerType.IsNotType<ITicker>())
                throw new ArgumentException($"{tickerType.GetName()} is not derived from {nameof(ITicker)}.");

            TickerType = tickerType;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Disposables;
using UTIRLib.Reflection;
using UTIRLib.Utils;

#nullable enable
namespace UTIRLib.Tickables
{
    public class Tickables : MonoXStatic<Tickables>
    {
        private readonly static NullReferenceException instanceException = new("Cannot find any ticker.");

        private readonly Dictionary<Type, ITicker> registeredTickers = new();

        protected override void OnStart()
        {
            base.OnStart();

            RegisterTickers();
            RegisterTickables();
        }

        public static bool IsTickerRegistered(ITicker? ticker)
        {
            Validate();

            if (ticker.IsNull())
                return false;

            return Instance.registeredTickers.ContainsKey(ticker.GetType());
        }

        public static bool IsTickableRegistered(ITickableBase? tickable)
        {
            Validate();

            if (tickable.IsNull())
                return false;
            if (!Tickable.TryGetTickerType(tickable, out Type? tickerType))
            {
                TirLibDebug.PrintWarning($"Tickable cannot be register by {nameof(Tickables)}. Return false.", Instance);
                return false;
            }

            if (!Instance.registeredTickers.TryGetValue(tickerType,
                                                        out ITicker? ticker)
                )
                return false;

            return ticker.IsRegistered(tickable);
        }

        /// <param name="ticker"></param>
        /// <returns>Disposable subscription</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IDisposable RegisterTicker(ITicker ticker)
        {
            Validate();

            if (ticker.IsNull())
                throw new ArgumentNullException(nameof(ticker));
            if (IsTickerRegistered(ticker))
                throw new ArgumentException($"{ticker.GetTypeName()} already registered.");

            Instance.registeredTickers.Add(ticker.GetType(), ticker);

            return new Subscription<ITicker>((x) => UnregisterTicker(x), ticker);
        }

        public static bool UnregisterTicker(ITicker? ticker)
        {
            Validate();

            if (ticker.IsNull())
                return false;

            return Instance.registeredTickers.Remove(ticker.GetType());
        }


        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IDisposable RegisterTickable(ITickableBase tickable)
        {
            Validate();

            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));
            if (!Tickable.TryGetTickerTypeAttribute(tickable, out TickerTypeAttribute? attribute))
                throw new ArgumentException($"{tickable.GetTypeName()} cannot be register by {nameof(Tickables)} due to lack of {nameof(TickerTypeAttribute)}. Use by ticker-instance registering.");
            if (!Instance.registeredTickers.TryGetValue(attribute.TickerType, out ITicker ticker))
                throw new ArgumentException($"Cannot find {attribute.TickerType.GetName()}");

            return ticker.Register(tickable);
        }

        public static bool UnregisterTickable(ITickableBase? tickable)
        {
            Validate();

            if (tickable.IsNull())
                return false;
            if (!Tickable.TryGetTickerTypeAttribute(tickable, out TickerTypeAttribute? attribute))
                throw new ArgumentException($"{tickable.GetTypeName()} cannot be unregsiter by {nameof(Tickables)} due to lack of {nameof(TickerTypeAttribute)}. Use by ticker-instance unregistering.");
            if (!Instance.registeredTickers.TryGetValue(attribute.TickerType, out ITicker ticker))
                throw new ArgumentException($"Cannot find {attribute.TickerType.GetName()}");

            return ticker.Unregister(tickable);
        }

        public static int RegisterTickers()
        {
            Validate();

            var tickers = UnityObjectHelper.FindObjectsByType<ITicker>(FindObjectsInactive.Include);

            if (tickers.IsEmpty())
                return 0;

            var tickersFiltered = tickers.Where(x => !IsTickerRegistered(x));

            int registeredCount = 0;
            foreach (var ticker in tickersFiltered)
            {
                RegisterTicker(ticker);
                registeredCount++;
            }

            return registeredCount;
        }

        public static int RegisterTickables()
        {
            Validate();

            var tickables = UnityObjectHelper.FindObjectsByType<ITickableBase>(FindObjectsInactive.Include);

            if (tickables.IsEmpty())
                return 0;

             var tickablesFiltered = from x in tickables
                                     select (x, attribute: x.GetType().GetCustomAttribute<TickerTypeAttribute>())
                                     into pair
                                     where pair.attribute != null && !IsTickableRegistered(pair.x)
                                     select pair.x;

            int registeredCount = 0;
            foreach (var tickable in tickablesFiltered)
            {
                RegisterTickable(tickable);
                registeredCount++;
            }

            return registeredCount;
        }

        private static void Validate()
        {
            if (Instance == null)
                throw instanceException;
        }
    }
}

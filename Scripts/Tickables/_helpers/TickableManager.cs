using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Disposables;
using UTIRLib.Reflection;
using UTIRLib.Unity.TypeMatching;
using UTIRLib.Utils;

#nullable enable
namespace UTIRLib.Tickables
{
    public class TickableManager : MonoX
    {
        private static TickableManager instance = null!;

        private readonly Dictionary<Type, ITicker> registeredTickers = new();

        protected override void OnAwake()
        {
            if (FindAnyObjectByType<TickableManager>().Is<TickableManager>())
                throw new ArgumentException($"On scene cannot be instantiated only one {nameof(TickableManager)}.");

            instance = this;
            base.OnAwake();
        }

        protected override void OnStart()
        {
            base.OnStart();

            ITicker[] tickers = UnityObjectHelper.FindObjectsByType<ITicker>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            foreach (var ticker in tickers)
                registeredTickers.Add(ticker.GetType(), ticker);

            RegisterTickables();
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

        public static bool IsTickerRegistered(ITicker? ticker)
        {
            if (ticker.IsNull())
                return false;

            return instance.registeredTickers.ContainsKey(ticker.GetType());
        }

        public static bool IsTickableRegistered(ITickableBase? tickable)
        {
            if (tickable.IsNull())
                return false;
            if (!TryGetTickerTypeAttribute(tickable, out TickerTypeAttribute? attribute))
            {
                TirLibDebug.PrintWarning($"Tickable cannot be register by {nameof(TickableManager)}. Return false.", instance);
                return false;
            }

            if (!instance.registeredTickers.TryGetValue(attribute.TickerType,
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
            if (ticker.IsNull())
                throw new ArgumentNullException(nameof(ticker));
            if (IsTickerRegistered(ticker))
                throw new ArgumentException($"{ticker.GetTypeName()} already registered.");

            instance.registeredTickers.Add(ticker.GetType(), ticker);

            return new Subscription<object, ITicker>((_, x) => UnregisterTicker(x), null!, ticker);
        }

        public static bool UnregisterTicker(ITicker? ticker)
        {
            if (ticker.IsNull())
                return false;

            return instance.registeredTickers.Remove(ticker.GetType());
        }


        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IDisposable RegisterTickable(ITickableBase tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));
            if (tickable.GetType().GetCustomAttribute(typeof(TickerTypeAttribute))
                .IsNot<TickerTypeAttribute>(out var attribute))
                throw new ArgumentException($"{tickable.GetTypeName()} cannot be register by {nameof(TickableManager)} due to lack of {nameof(TickerTypeAttribute)}. Use by ticker-instance registering.");
            if (!instance.registeredTickers.TryGetValue(attribute.TickerType, out ITicker ticker))
                throw new ArgumentException($"Cannot find {attribute.TickerType.GetName()}");

            return ticker.Register(tickable);
        }

        public static bool UnregisterTickable(ITickableBase? tickable)
        {
            if (tickable.IsNull())
                return false;
            if (tickable.GetType().GetCustomAttribute(typeof(TickerTypeAttribute))
                .IsNot<TickerTypeAttribute>(out var attribute))
                throw new ArgumentException($"{tickable.GetTypeName()} cannot be unregsiter by {nameof(TickableManager)} due to lack of {nameof(TickerTypeAttribute)}. Use by ticker-instance unregistering.");
            if (!instance.registeredTickers.TryGetValue(attribute.TickerType, out ITicker ticker))
                throw new ArgumentException($"Cannot find {attribute.TickerType.GetName()}");

            return ticker.Unregister(tickable);
        }

        public static int RegisterTickers()
        {
            var tickers = UnityObjectHelper.FindObjectsByType<ITicker>(FindObjectsInactive.Include);

            if (tickers.IsEmpty())
                return 0;

            var tickersFiltered = from x in tickers
                                  where !IsTickerRegistered(x)
                                  select x;

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
    }
}

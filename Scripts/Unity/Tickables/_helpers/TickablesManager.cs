using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Reflection;
using CCEnvs.Unity.Attributes;
using CCEnvs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Tickables
{
    public sealed class TickablesManager : MonoCCStatic<TickablesManager>
    {
        private readonly static NullReferenceException instanceException = new("Cannot find any ticker.");

        private readonly Dictionary<Type, ITicker> registeredTickers = new();

        protected override void OnAwake()
        {
            base.OnAwake();

            DontDestroyOnLoad(gameObject);
            //OnInstantiated += TryRegisterTickable;
        }

        protected override void OnStart()
        {
            base.OnStart();

            RegisterTickers();
            RegisterTickables();
        }

        public static bool IsTickerRegistered(ITicker? ticker)
        {
            if (ticker.IsNull())
                return false;

            return Instance.registeredTickers.ContainsKey(ticker.GetType());
        }

        public static bool IsTickableRegistered(ITickableBase? tickable)
        {
            if (tickable.IsNull())
                return false;
            if (!Tickable.TryGetTickerType(tickable, out Type? tickerType))
            {
                CCDebug.PrintWarning($"Tickable cannot be register by {nameof(TickablesManager)}. Return false.", Instance);
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
            if (ticker.IsNull())
                throw new ArgumentNullException(nameof(ticker));
            if (IsTickerRegistered(ticker))
                throw new ArgumentException($"{ticker.GetTypeName()} already registered.");

            Instance.registeredTickers.Add(ticker.GetType(), ticker);

            return Subscription.Create(ticker, (x) => UnregisterTicker(x));
        }

        public static bool UnregisterTicker(ITicker? ticker)
        {
            if (ticker.IsNull())
                return false;

            return Instance.registeredTickers.Remove(ticker.GetType());
        }

        public static IDisposable RegisterTickable(ITickableBase tickable, Type tickerType)
        {
            CC.Validate.ArgumentNull(tickable, nameof(tickable));
            CC.Validate.ArgumentNull(tickerType, nameof(tickerType));

            if (!Instance.registeredTickers.TryGetValue(tickerType, out ITicker? ticker))
                throw new LogicException($"Cannot find ticker of type: {tickerType.GetName()}.");

            return ticker.Register(tickable);
        }

        public static IDisposable RegisterTickable(ITickableBase tickable)
        {
            CC.Validate.ArgumentNull(tickable, nameof(tickable));
            if (!Tickable.TryGetTickerType(tickable, out Type? tickerType))
                throw new ArgumentException($"{tickable.GetType()} cannot be implcitly registered in {nameof(TickablesManager)}.");

            return RegisterTickable(tickable, tickerType);
        }

        public static bool UnregisterTickable(ITickableBase? tickable)
        {
            if (tickable.IsNull())
                return false;
            if (!Tickable.TryGetTickerType(tickable, out Type? tickerType))
                throw new ArgumentException($"Tickable cannot be register by {nameof(TickablesManager)}.");
            if (!Instance.registeredTickers.TryGetValue(tickerType, out ITicker ticker))
                throw new ArgumentException($"Cannot find {tickerType.GetName()}");

            return ticker.Unregister(tickable);
        }

        public static int RegisterTickers()
        {
            var tickers = UObjectFinder.FindObjectsByType<ITicker>(FindObjectsInactive.Include);

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
            var tickables = UObjectFinder.FindObjectsByType<ITickableBase>(FindObjectsInactive.Include);

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

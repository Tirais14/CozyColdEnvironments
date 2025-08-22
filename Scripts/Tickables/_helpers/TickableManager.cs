using System;
using System.Collections.Generic;
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
        }

        public static bool IsTickerRegistered(ITicker? ticker)
        {
            if (ticker.IsNull())
                return false;

            return instance.registeredTickers.ContainsKey(ticker.GetType());
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

        public static void UnregisterTicker(ITicker? ticker)
        {
            if (ticker.IsNull())
                return;

            instance.registeredTickers.Remove(ticker.GetType());
        }

        public static IDisposable RegisterTickable(ITickableBase tickable)
        {
            if (tickable.IsNull())
                throw new ArgumentNullException(nameof(tickable));
            if (!instance.registeredTickers.TryGetValue(tickable.GetType(), out ITicker ticker))
                throw new ArgumentException($"Cannot fin {tickable.GetTypeName()}");

            ticker.Re
        }
    }
}

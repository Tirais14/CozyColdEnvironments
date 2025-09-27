using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using CCEnvs.Utils;
using LinqAF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
namespace CCEnvs.Unity.Tickables
{
    public sealed class TickablesManager : CCBehaviourStatic<TickablesManager>
    {
        private readonly Dictionary<Type, ITicker> tickers = new();

        protected override void OnStart()
        {
            base.OnStart();

            RegisterTickers();
            RegisterTickables();

            SceneManager.sceneLoaded += (_, _) => RegisterTickables();
        }

        public static bool IsTickerRegistered(ITicker? ticker)
        {
            if (ticker.IsNull())
                return false;

            return Self.tickers.ContainsKey(ticker.GetType());
        }

        /// <exception cref="CannotRegisterTickerException"></exception>
        public static IDisposable RegisterTicker(ITicker ticker)
        {
            CC.Validate.ArgumentNull(ticker, nameof(ticker));

            if (IsTickerRegistered(ticker))
                throw new CannotRegisterTickerException(
                    "Already registered.",
                    ticker.GetType());

            Self.tickers.Add(ticker.GetType(), ticker);
            CCDebug.PrintLog($"Registered ticker: {ticker.GetType().GetName()}.");

            return Subscription.Create(ticker, (x) => UnregisterTicker(x));
        }

        public static bool UnregisterTicker(ITicker? ticker)
        {
            if (ticker.IsNull())
                return false;

            return Self.tickers.Remove(ticker.GetType());
        }

        public static bool IsTickableRegistered<T>(T? tickable)
            where T : ITickableBase
        {
            if (tickable.IsNull()
                ||
                !Tickable.TryGetTickerType(tickable, out Type? tickerType)
                ||
                !Self.tickers.TryGetValue(tickerType, out ITicker? ticker))
                return false;

            return ticker.IsRegistered(tickable);
        }

        /// <exception cref="CannotRegisterTickableException"></exception>
        public static IDisposable RegisterTickable<T>(T tickable, Type tickerType)
            where T : ITickableBase
        {
            CC.Validate.ArgumentNull(tickable, nameof(tickable));
            CC.Validate.ArgumentNull(tickerType, nameof(tickerType));

            if (!Self.tickers.TryGetValue(tickerType, out ITicker? ticker))
                throw new CannotRegisterTickableException(
                    $"Not found ticker: {tickerType.GetName()}.",
                    tickable.GetType());

            IDisposable result = ticker.Register(tickable);
            CCDebug.PrintLog($"Registered tickable: {tickable.GetType().GetFullName()} to {tickerType.GetName()}.");

            return result;
        }

        public static IDisposable RegisterTickable<T>(T tickable)
            where T : ITickableBase
        {
            CC.Validate.ArgumentNull(tickable, nameof(tickable));

            if (!Tickable.TryGetTickerType(tickable, out Type? tickerType))
                throw new CannotRegisterTickableException(
                    "Cannot resolve ticker type.",
                    tickable.GetType());

            return RegisterTickable(tickable, tickerType);
        }

        public static bool UnregisterTickable<T>(T tickable, Type tickerType)
            where T : ITickableBase
        {
            CC.Validate.ArgumentNull(tickable, nameof(tickable));
            CC.Validate.ArgumentNull(tickerType, nameof(tickerType));

            var result = Self.tickers[tickerType].Unregister(tickable);
            CCDebug.PrintLog($"Unregistered tickable: {tickable.GetType().GetFullName()} from {tickerType.GetName()}.");

            return result;
        }

        public static bool UnregisterTickable<T>(T? tickable)
            where T : ITickableBase
        {
            CC.Validate.ArgumentNull(tickable, nameof(tickable));

            if (!Tickable.TryGetTickerType(tickable, out Type? tickerType))
                return false;

            return UnregisterTickable(tickable, tickerType);
        }

        public static int RegisterTickers()
        {
            CCDebug.PrintLog("Registering tickers started.", Self);
            var stopwatch = new Stopwatch();

            WhereEnumerable<ITicker, IdentityEnumerable<ITicker, ITicker[], ArrayBridger<ITicker>, ArrayEnumerator<ITicker>>, ArrayEnumerator<ITicker>> tickersFiltered = SceneObjectSearch.FindObjectsByType<ITicker>(FindObjectsInactive.Include)
                .Where(ticker => !IsTickerRegistered(ticker));

            int registeredCount = 0;
            foreach (ITicker? ticker in tickersFiltered)
            {
                RegisterTicker(ticker);
                registeredCount++;
            }

            CCDebug.PrintLog($"Registering tickers finished in {stopwatch.Elapsed.TotalSeconds} seconds. Count: {registeredCount}.", Self);
            return registeredCount;
        }

        public static int RegisterTickables()
        {
            CCDebug.PrintLog("Registering tickables started.", Self);
            var stopwatch = new Stopwatch();

            SelectWhereEnumerable<(ITickableBase tickable, bool state, Type tickerType), ITickableBase, IdentityEnumerable<ITickableBase, ITickableBase[], ArrayBridger<ITickableBase>, ArrayEnumerator<ITickableBase>>, ArrayEnumerator<ITickableBase>, SingleProjection<(ITickableBase tickable, bool state, Type tickerType), ITickableBase>, SinglePredicate<(ITickableBase tickable, bool state, Type tickerType)>> toRegister =
               from tickable in SceneObjectSearch.FindObjectsByType<ITickableBase>(FindObjectsInactive.Include)
               select (tickable, state: Tickable.TryGetTickerType(tickable, out Type? tickerType), tickerType) into item
               where item.state && !IsTickableRegistered(item.tickable)
               select item;

            int registeredCount = 0;
            foreach ((ITickableBase tickable, bool _, Type tickerType) in toRegister)
            {
                RegisterTickable(tickable, tickerType);
                registeredCount++;
            }

            CCDebug.PrintLog($"Registering tickables finished in {stopwatch.Elapsed.TotalSeconds} seconds. Count: {registeredCount}.", Self);
            return registeredCount;
        }
    }
}

#nullable enable
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

namespace CCEnvs
{
    public static class Do
    {
        public delegate T?[] MoveNext<T>(T current, LoopState loopState);

        public static void Nothing()
        {
        }

        public static T? Nothing<T>() => default;

        /// <summary>
        /// Same as the recursion loop, but use heap memory
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<T> Collect<T>(T first, MoveNext<T> moveNext)
        {
            if (moveNext is null)
                throw new ArgumentNullException(nameof(moveNext));

            var toProccess = new Queue<T>();
            toProccess.Enqueue(first);

            var results = new Queue<T>();

            var loopState = new LoopState();
            T?[] nextValues;
            T? current;
            var loopPredicate = new LoopFuse(() => toProccess.Count > 0);
            while (loopPredicate.Invoke())
            {
                current = toProccess.Dequeue();
                if (current is null)
                    continue;

                nextValues = moveNext(current, loopState);
                results.Enqueue(current);

                if (loopState.Break)
                    break;
                else if (loopState.Continue.Use())
                    continue;

                int nextValuesCount = nextValues.Length;
                for (int i = 0; i < nextValuesCount; i++)
                {
                    if (nextValues[i] is not null)
                        toProccess.Enqueue(nextValues[i]!);
                }
            }

            return results;
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<T> Collect<T>(T first, Func<T, LoopState, T?> moveNext)
        {
            if (moveNext is null)
                throw new ArgumentNullException(nameof(moveNext));

            return Collect(first, (x, loopState) => new T?[] { moveNext(x, loopState) });
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<T> Collect<T>(T first, Func<T, T?[]> moveNext)
        {
            if (moveNext is null)
                throw new ArgumentNullException(nameof(moveNext));

            return Collect(first, (x, _) => moveNext(x));
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<T> Collect<T>(T first, Func<T, T?> moveNext)
        {
            if (moveNext is null)
                throw new ArgumentNullException(nameof(moveNext));

            return Collect(first, (x, _) => new T?[] { moveNext(x) });
        }

        public static void While(
            Func<bool> predicate,
            Action<LoopState>? action,
            ulong iterationsLimit = LoopChecker.ITERATIONS_LIMIT_DEFAULT,
            bool throwOnLimit = true)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (action is null)
                return;

            var loopFuse = new LoopFuse(predicate)
            {
                IterationsLimit = iterationsLimit
            };
            var state = new LoopState();
            if (!throwOnLimit)
            {
                try
                {
                    while (loopFuse)
                    {
                        if (state.Break)
                            break;

                        if (state.Continue.Use())
                            continue;

                        action(state);
                    }
                }
                catch (EndlessLoopException ex)
                {
                    typeof(Do).PrintException(ex);
                }
            }
            else
            {
                while (loopFuse)
                {
                    if (state.Break)
                        break;

                    if (state.Continue.Use())
                        continue;

                    action(state);
                }
            }
        }
        public static void While(
            Func<bool> predicate,
            Action? action,
            ulong iterationsLimit = LoopChecker.ITERATIONS_LIMIT_DEFAULT,
            bool throwOnLimit = true)
        {
            if (action is null)
                return;

            While(predicate, _ => action(), iterationsLimit, throwOnLimit);
        }

        public static T[] While<T>(
            Func<bool> predicate,
            Func<T>? action,
            ulong iterationsLimit = LoopChecker.ITERATIONS_LIMIT_DEFAULT,
            bool throwOnLimit = true)
        {
            Guard.IsNotNull(predicate, nameof(predicate));

            if (action is null)
                return Array.Empty<T>();

            var results = new List<T>();
            var loopFuse = new LoopFuse(predicate)
            {
                IterationsLimit = iterationsLimit
            };
            if (!throwOnLimit)
            {
                try
                {
                    while (loopFuse)
                        results.Add(action());
                }
                catch (EndlessLoopException ex)
                {
                    typeof(Do).PrintException(ex);
                }
            }
            else
                while (loopFuse)
                    results.Add(action());

            return results.ToArray();
        }
    }
}

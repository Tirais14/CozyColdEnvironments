#nullable enable
using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using R3;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CCEnvs
{
    public static class Do
    {
        public delegate T?[] MoveNext<T>(T current, LoopState loopState);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeSubject<T>(ref Subject<T>? subj)
        {
            if (subj is null)
                return;

            try
            {
                subj.OnCompleted();
            }
            catch (Exception ex)
            {
                subj.OnErrorResume(ex);
            }

            subj.Dispose();
            subj = null!;
        }

        public static object? ReturnNull() => null;

        public static T? ReturnDefault<T>() => default;

        public static bool TryFindHoleInRange(int start, int count, IEnumerable<int> range, out int hole)
        {
            if (range.IsEmpty() || start == count - 1)
            {
                hole = default;
                return false;
            }

            var zipped = Enumerable.Range(start, count).EquiZip(range.OrderBy(x => x));

            foreach (var pair in zipped)
            {
                if (pair.Item1 != pair.Item2)
                {
                    hole = pair.Item1;
                    return true;
                }
            }

            hole = default;
            return false;
        }

        public static bool Compare(int value, CompareTypes compareTypes)
        {
            if (compareTypes.IsFlagSetted(CompareTypes.Equals))
            {
                if (compareTypes.IsFlagSetted(CompareTypes.Smaller))
                    return value <= 0;
                else if (compareTypes.IsFlagSetted(CompareTypes.Bigger))
                    return value >= 0;
                else
                    return value == 0;
            }
            else if (compareTypes.IsFlagSetted(CompareTypes.Smaller))
                return value < 0;
            else if (compareTypes.IsFlagSetted(CompareTypes.Bigger))
                return value > 0;

            throw CC.ThrowHelper.InvalidOperationException(compareTypes, nameof(compareTypes));
        }

        public static bool CompareTo<T>(this T left, T right, CompareTypes compareTypes)
            where T : IComparable<T>
        {
            CC.Guard.IsNotNull(left, nameof(left));
            CC.Guard.IsNotNull(right, nameof(right));

            return Compare(left.CompareTo(right), compareTypes);
        }

        public static bool CompareTo<T>(this T left,
            T right,
            CompareTypes compareTypes,
            IComparer<T> comparer)
        {
            CC.Guard.IsNotNull(comparer, nameof(comparer));

            return Compare(comparer.Compare(left, right), compareTypes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Nothing()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Nothing<T>(T _)
        {
        }

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
                else if (loopState.Continue.GetValue())
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

                        if (state.Continue.GetValue())
                            continue;

                        action(state);
                    }
                }
                catch (InvalidOperationException ex)
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

                    if (state.Continue.GetValue())
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
                catch (InvalidOperationException ex)
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

using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs
{
    public static class Collector
    {
        public delegate T?[] MoveNext<T>(T current, LoopState loopState);

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

                if (loopState.Value == LoopKeyword.Break)
                {
                    loopState.Reset();
                    break;
                }
                else if (loopState.Value == LoopKeyword.Continue)
                {
                    loopState.Reset();
                    continue;
                }

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
    }
}

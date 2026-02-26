using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static CCEnvs.Do;

#nullable enable
namespace CCEnvs
{
    public static class Loops
    {
        /// <summary>
        /// Same as the recursion loop, but use heap memory
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<T> BreadthFirstSearch<T>(T first, MoveNext<T> moveNext)
        {
            if (moveNext is null)
                throw new ArgumentNullException(nameof(moveNext));

            var toProcess = new Queue<T>();
            toProcess.Enqueue(first);

            var results = new Queue<T>();

            var loopState = new LoopState();

            var loopFuse = LoopFuse.Create();

            IEnumerable<T?>? nextValues = null;
            T? current;

            while (toProcess.Count > 0 && loopFuse.DebugMoveNext())
            {
                current = toProcess.Dequeue();

                if (current is null)
                    continue;

                results.Enqueue(current);

                try
                {
                    nextValues = moveNext(current, loopState);

                    if (loopState.Break)
                        break;
                    else if (loopState.Continue.GetValue())
                        continue;

                    ScheduleNextValues(toProcess, nextValues);
                }
                catch (Exception ex)
                {
                    current.PrintException(ex);
                    continue;
                }
                finally
                {
                    if (nextValues is IDisposable disposable)
                        disposable.Dispose();
                }
                
            }

            return results;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void ScheduleNextValues(Queue<T> toProcess, IEnumerable<T?> nextValues)
            {
                foreach (var nextValue in nextValues)
                    toProcess.Enqueue(nextValue!);
            }
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<T> BreadthFirstSearch<T>(T first, Func<T, LoopState, T?> moveNext)
        {
            if (moveNext is null)
                throw new ArgumentNullException(nameof(moveNext));

            return BreadthFirstSearch(first, (x, loopState) => new T?[] { moveNext(x, loopState) });
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<T> BreadthFirstSearch<T>(T first, Func<T, T?[]> moveNext)
        {
            if (moveNext is null)
                throw new ArgumentNullException(nameof(moveNext));

            return BreadthFirstSearch(first, (x, _) => moveNext(x));
        }
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<T> BreadthFirstSearch<T>(T first, Func<T, T?> moveNext)
        {
            if (moveNext is null)
                throw new ArgumentNullException(nameof(moveNext));

            return BreadthFirstSearch(first, (x, _) =>
            {
                var values = ArrayPool<T?>.Shared.Get(1);

                values[0] = moveNext(x);

                return values;
            });
        }

        public static void
    }
}

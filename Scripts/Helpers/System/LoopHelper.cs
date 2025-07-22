using System;
using System.Collections.Generic;
using static UTIRLib.LoopHelper;

#nullable enable
namespace UTIRLib
{
    public static class LoopHelper
    {
        public delegate T MoveNext<T>(T current, Stack<T> toProccess);

        /// <summary>
        /// Same as the recursion loop, but use heap memory
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static Queue<T> Collect<T>(T first,
                                          MoveNext<T> moveNext)
        {
            if (moveNext is null)
                throw new ArgumentNullException(nameof(moveNext));

            var toProccess = new Stack<T>();
            var results = new Queue<T>();

            T current = first;
            var loopPredicate = new LoopPredicate(() => toProccess.Count > 0);
            while (loopPredicate.Invoke())
            {
                current = moveNext(current, toProccess);

                results.Enqueue(current);
            }

            return results;
        }
    }
}

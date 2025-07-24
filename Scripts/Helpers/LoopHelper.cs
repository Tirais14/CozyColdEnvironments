using System;
using System.Collections.Generic;

#nullable enable
namespace UTIRLib
{
    public static class LoopHelper
    {
        public delegate LoopIteration<T[]> MoveNext<T>(T current);

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
            toProccess.Push(first);

            var results = new Queue<T>();

            LoopIteration<T[]> iteration;
            var loopPredicate = new LoopPredicate(() => toProccess.Count > 0);
            while (loopPredicate.Invoke())
            {
                iteration = moveNext(toProccess.Pop());

                if (iteration.Keyword == LoopKeyword.Break)
                    break;
                else if (iteration.Keyword == LoopKeyword.Continue)
                    continue;

                for (int i = 0; i < iteration.Value.Length; i++)
                    results.Enqueue(iteration.Value[i]);
            }

            return results;
        }
    }
}

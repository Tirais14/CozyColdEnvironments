using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace UTIRLib
{
    public struct Collection<T> : IEnumerator<T>, IEnumerable<T>
    {
        private readonly Predicate<T?> moveNext;
        private readonly Func<T?> getNext;
        private readonly ulong iterationsLimit;
        private ulong iterations;

        public T Current { get; private set; }

        readonly object IEnumerator.Current => Current!;

        public Collection(Predicate<T?> moveNext,
                          Func<T?> getNext,
                          ulong iterationsLimit = LoopChecker.ITERATIONS_LIMIT_DEFAULT)
        {
            this.moveNext = moveNext;
            this.getNext = getNext;
            this.iterationsLimit = iterationsLimit;
            iterations = 0uL;

            Current = default!;
        }

        /// <exception cref="EndlessLoopException"></exception>
        public bool MoveNext()
        {
            iterations++;

            if (iterations >= iterationsLimit)
                throw new EndlessLoopException(iterations);

            Current = getNext()!;

            return moveNext(Current);
        }

        public readonly void Reset()
        {
        }

        public readonly void Dispose()
        {
        }

        public readonly Collection<T> GetEnumerator() => this;

        readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

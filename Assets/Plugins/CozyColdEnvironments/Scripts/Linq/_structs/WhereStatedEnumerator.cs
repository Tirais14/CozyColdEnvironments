using System;
using System.Collections;
using System.Collections.Generic;
using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Linq
{
    public struct WhereStatedEnumerator<T, TState>
        :
        IEnumerator<T>,
        IEnumerable<T>
    {
        private readonly TState state;

        private readonly Func<T, TState, bool> predicate;

        private readonly IEnumerable<T> source;

        private IEnumerator<T>? enumerator;

        public WhereStatedEnumerator(
            IEnumerable<T> source,
            TState state,
            Func<T, TState, bool> predicate
            )
            :
            this()
        {
            CC.Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(predicate, nameof(predicate));

            this.state = state;
            this.predicate = predicate;
        }

        public T Current { get; private set; }

        readonly object IEnumerator.Current => Current!;

        public bool MoveNext()
        {
            enumerator ??= source.GetEnumerator();

            while (enumerator.TryMoveNext(out var tCurrent))
            {
                if (!predicate(tCurrent, state))
                    continue;

                Current = tCurrent;

                return true;
            }

            return false;
        }

        public readonly void Reset()
        {
            throw new NotSupportedException(nameof(Reset));
        }

        public readonly void Dispose()
        {
        }

        public readonly IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

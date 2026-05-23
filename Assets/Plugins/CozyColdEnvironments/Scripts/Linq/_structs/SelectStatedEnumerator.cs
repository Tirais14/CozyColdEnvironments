using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Linq
{
    public struct SelectStatedEnumerator<T, TState, TOut>
        :
        IEnumerator<TOut>,
        IEnumerable<TOut>
    {
        private readonly IEnumerable<T> source;

        private readonly TState state;

        private readonly Func<T, TState, TOut> converter;

        private IEnumerator<T>? enumerator;

        public TOut Current { readonly get; private set; }

        readonly object IEnumerator.Current => Current!;

        public SelectStatedEnumerator(
            IEnumerable<T> source,
            TState state,
            Func<T, TState, TOut> converter
            )
            :
            this()
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(converter, nameof(converter));

            this.source = source;
            this.state = state;
            this.converter = converter;
        }

        public bool MoveNext()
        {
            enumerator ??= source.GetEnumerator();

            if (!enumerator.TryMoveNext(out var tCurrent))
                return false;

            Current = converter(tCurrent, state);

            return true;
        }

        public void Reset()
        {
            throw new System.NotSupportedException(nameof(Reset));
        }

        public void Dispose()
        {
        }

        public IEnumerator<TOut> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

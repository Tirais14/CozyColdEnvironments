using CommunityToolkit.Diagnostics;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public sealed class AnonymousDragPredicate : IDragPredicate
    {
        private Func<bool> predicate;

        public AnonymousDragPredicate(Func<bool> predicate)
        {
            Guard.IsNotNull(predicate);
            this.predicate = predicate;
        }

        public bool Evaluate() => predicate();
    }

    public sealed class AnonymousDragPredicate<TState> : IDragPredicate
    {
        private TState state;
        private Func<TState, bool> predicate;

        public AnonymousDragPredicate(TState state, Func<TState, bool> predicate)
        {
            Guard.IsNotNull(predicate);
            this.state = state; 
            this.predicate = predicate;
        }

        public bool Evaluate() => predicate(state);
    }
}

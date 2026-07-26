using System;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public static class DragPredicate
    {
        public static IDragPredicate True { get; } = Create(() => true);
        public static IDragPredicate False { get; } = Create(() => false);

        public static IDragPredicate Create(Func<bool> predicate)
        {
            return new AnonymousDragPredicate(predicate);
        }
        public static IDragPredicate Create<TState>(TState state, Func<TState, bool> predicate)
        {
            return new AnonymousDragPredicate<TState>(state, predicate);
        }
    }
}

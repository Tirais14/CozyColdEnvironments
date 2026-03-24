#nullable enable
namespace CCEnvs.Patterns.States
{
    public static class StateTransitionPredicate
    {
        public static IStateTransitionPredicate True { get; } = new AnonymousStateTransitionPredicate(() => true);
        public static IStateTransitionPredicate False { get; } = new AnonymousStateTransitionPredicate(() => false);
    }
}

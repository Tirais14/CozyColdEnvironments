#nullable enable
namespace CCEnvs.Patterns.States
{
    public interface IStateTransitionPredicate
    {
        bool Evaluate();
    }
}

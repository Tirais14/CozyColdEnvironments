using UTIRLib.Patterns.Composite;

namespace UTIRLib.Animation
{
    public class CompositeAnimatorState : Composite<IAnimatorState>, IComposite<IAnimatorState>
    {
        public CompositeAnimatorState(params IAnimatorState[] states) : base(states)
        { }
    }
}
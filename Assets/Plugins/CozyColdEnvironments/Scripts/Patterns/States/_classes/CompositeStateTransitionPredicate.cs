using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Patterns.States
{
    public class CompositeStateTransitionPredicate : IStateTransitionPredicate
    {
        private readonly List<IStateTransitionPredicate> predicates = new();

        public CompositeStateTransitionPredicate()
        {

        }

        public CompositeStateTransitionPredicate(IEnumerable<IStateTransitionPredicate> predicates)
        {
            AddRange(predicates);
        }

        public CompositeStateTransitionPredicate Add(IStateTransitionPredicate predicate)
        {
            CC.Guard.IsNotNull(predicate, nameof(predicate));

            predicates.Add(predicate);
            return this;
        }

        public CompositeStateTransitionPredicate AddRange(IEnumerable<IStateTransitionPredicate> predicates)
        {
            CC.Guard.IsNotNull(predicates, nameof(predicates));

            this.predicates.AddRange(predicates);
            return this;
        }

        public CompositeStateTransitionPredicate Remove(IStateTransitionPredicate predicate)
        {
            CC.Guard.IsNotNull(predicate, nameof(predicate));

            predicates.Remove(predicate);
            return this;
        }

        public bool Evaluate()
        {
            for (int i = 0; i < predicates.Count; i++)
            {
                if (!predicates[i].Evaluate())
                    return false;
            }

            return true;
        }
    }
}

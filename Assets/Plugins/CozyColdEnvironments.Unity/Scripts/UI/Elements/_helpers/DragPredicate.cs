using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static IDragPredicate Create(params IDragPredicate?[] predicates)
        {
            Guard.IsNotNull(predicates);

            int firstPredicateIdx = 0;
            int predicateCount = 0;

            for (int i = 0; i < predicates.Length; i++)
            {
                if (predicates[i].IsNotNull())
                {
                    predicateCount++;

                    if (predicateCount == 0)
                        firstPredicateIdx = i;
                }
            }

            if (predicateCount == 1)
            {
                if (predicates[firstPredicateIdx].IsNull())
                    return True;

                return new AnonymousDragPredicate<IDragPredicate>(
                    predicates[firstPredicateIdx]!,
                    predicate => predicate.Evaluate()
                    );
            }

            return new AnonymousDragPredicate<IDragPredicate[]>(
                predicates.Where(predicate => predicate.IsNotNull()).ToArray()!,
                static (predicates) =>
                {
                    foreach (var predicate in predicates)
                        if (!predicate.Evaluate())
                            return false;

                    return true;
                });
        }
    }
}

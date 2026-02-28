#nullable enable
using CCEnvs.Collections;
using R3;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CCEnvs
{
    public static class Do
    {
        public delegate IEnumerable<T?> MoveNext<T>(T current, LoopState loopState);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeSubject<T>(ref Subject<T>? subj)
        {
            if (subj is null)
                return;

            try
            {
                subj.OnCompleted();
            }
            catch (Exception ex)
            {
                subj.OnErrorResume(ex);
            }

            subj.Dispose();
            subj = null!;
        }

        public static object? ReturnNull() => null;

        public static T? ReturnDefault<T>() => default;

        public static bool TryFindHoleInRange(int start, int count, IEnumerable<int> range, out int hole)
        {
            if (range.IsEmpty() || start == count - 1)
            {
                hole = default;
                return false;
            }

            var zipped = Enumerable.Range(start, count).EquiZip(range.OrderBy(x => x));

            foreach (var pair in zipped)
            {
                if (pair.Item1 != pair.Item2)
                {
                    hole = pair.Item1;
                    return true;
                }
            }

            hole = default;
            return false;
        }

        public static bool Compare(int value, CompareTypes compareTypes)
        {
            if (compareTypes.IsFlagSetted(CompareTypes.Equals))
            {
                if (compareTypes.IsFlagSetted(CompareTypes.Smaller))
                    return value <= 0;
                else if (compareTypes.IsFlagSetted(CompareTypes.Bigger))
                    return value >= 0;
                else
                    return value == 0;
            }
            else if (compareTypes.IsFlagSetted(CompareTypes.Smaller))
                return value < 0;
            else if (compareTypes.IsFlagSetted(CompareTypes.Bigger))
                return value > 0;

            throw CC.ThrowHelper.InvalidOperationException(compareTypes, nameof(compareTypes));
        }

        public static bool CompareTo<T>(this T left, T right, CompareTypes compareTypes)
            where T : IComparable<T>
        {
            CC.Guard.IsNotNull(left, nameof(left));
            CC.Guard.IsNotNull(right, nameof(right));

            return Compare(left.CompareTo(right), compareTypes);
        }

        public static bool CompareTo<T>(this T left,
            T right,
            CompareTypes compareTypes,
            IComparer<T> comparer)
        {
            CC.Guard.IsNotNull(comparer, nameof(comparer));

            return Compare(comparer.Compare(left, right), compareTypes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Nothing()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Nothing<T>(T _)
        {
        }
    }
}

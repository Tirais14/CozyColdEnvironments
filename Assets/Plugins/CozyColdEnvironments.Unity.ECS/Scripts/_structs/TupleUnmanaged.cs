#nullable enable
using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;

namespace CCEnvs.UnityX.ECS
{
    public readonly struct TupleUnmanaged
    {
        [BurstCompile]
        public static TupleUnmanaged<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
            where T1 : struct
            where T2 : struct
        {
            return new TupleUnmanaged<T1, T2>
            {
                Item1 = item1,
                Item2 = item2
            };
        }

        [BurstCompile]
        public static TupleUnmanaged<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
            where T1 : struct
            where T2 : struct
            where T3 : struct
        {
            return new TupleUnmanaged<T1, T2, T3>
            {
                Item1 = item1,
                Item2 = item2,
                Item3 = item3
            };
        }
    }

    public struct TupleUnmanaged<T1, T2>
        :
        IEquatable<TupleUnmanaged<T1, T2>>

        where T1 : struct
        where T2 : struct
    {
        public T1 Item1;
        public T2 Item2;

        [BurstCompile]
        public static bool operator ==(TupleUnmanaged<T1, T2> left, TupleUnmanaged<T1, T2> right)
        {
            return left.Equals(right);
        }

        [BurstCompile]
        public static bool operator !=(TupleUnmanaged<T1, T2> left, TupleUnmanaged<T1, T2> right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TupleUnmanaged<T1, T2> unmanaged && Equals(unmanaged);
        }

        public readonly bool Equals(TupleUnmanaged<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
                   EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
        }

        [BurstCompile]
        public readonly override int GetHashCode()
        {
            return (int)math.hash(new int2(Item1.GetHashCode(), Item2.GetHashCode()));
        }
    }

    public struct TupleUnmanaged<T1, T2, T3> 
        :
        IEquatable<TupleUnmanaged<T1, T2, T3>>

        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        [BurstCompile]
        public static bool operator ==(TupleUnmanaged<T1, T2, T3> left, TupleUnmanaged<T1, T2, T3> right)
        {
            return left.Equals(right);
        }

        [BurstCompile]
        public static bool operator !=(TupleUnmanaged<T1, T2, T3> left, TupleUnmanaged<T1, T2, T3> right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TupleUnmanaged<T1, T2, T3> unmanaged && Equals(unmanaged);
        }

        public readonly bool Equals(TupleUnmanaged<T1, T2, T3> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1) &&
                   EqualityComparer<T2>.Default.Equals(Item2, other.Item2) &&
                   EqualityComparer<T3>.Default.Equals(Item3, other.Item3);
        }

        [BurstCompile]
        public readonly override int GetHashCode()
        {
            return (int)math.hash(new int3(Item1.GetHashCode(), Item2.GetHashCode(), Item3.GetHashCode()));
        }
    }
}

#nullable enable
using System;
using Unity.Burst;
using Unity.Mathematics;

namespace CCEnvs.UnityX.ECS
{
    public readonly struct Tpl
    {
        [BurstCompile]
        public static Tpl<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
            where T1 : unmanaged
            where T2 : unmanaged
        {
            return new Tpl<T1, T2>
            {
                Item1 = item1,
                Item2 = item2
            };
        }

        [BurstCompile]
        public static TupleUnmanaged<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
            where T1 : unmanaged
            where T2 : unmanaged
            where T3 : unmanaged
        {
            return new TupleUnmanaged<T1, T2, T3>
            {
                Item1 = item1,
                Item2 = item2,
                Item3 = item3
            };
        }
    }

    public struct Tpl<T1, T2>
        :
        IEquatable<Tpl<T1, T2>>

        where T1 : unmanaged
        where T2 : unmanaged
    {
        public T1 Item1;
        public T2 Item2;

        [BurstCompile]
        public readonly void Deconstruct(out T1 item1, out T2 item2)
        {
            item1 = Item1;
            item2 = Item2;
        }

        [BurstCompile]
        public static bool operator ==(Tpl<T1, T2> left, Tpl<T1, T2> right)
        {
            return left.Equals(right);
        }

        [BurstCompile]
        public static bool operator !=(Tpl<T1, T2> left, Tpl<T1, T2> right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is Tpl<T1, T2> unmanaged && Equals(unmanaged);
        }

        [BurstCompile]
        public readonly bool Equals(Tpl<T1, T2> other)
        {
            return Item1.GetHashCode() == other.Item1.GetHashCode()
                   &&
                   Item2.GetHashCode() == other.Item2.GetHashCode();
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

        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        [BurstCompile]
        public readonly void Deconstruct(out T1 item1, out T2 item2, out T3 item3)
        {
            item1 = Item1;
            item2 = Item2;
            item3 = Item3;
        }

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

        [BurstCompile]
        public readonly bool Equals(TupleUnmanaged<T1, T2, T3> other)
        {
            return Item1.GetHashCode() == Item1.GetHashCode()
                   &&
                   Item2.GetHashCode() == Item2.GetHashCode()
                   &&
                   Item3.GetHashCode() == Item3.GetHashCode();
        }

        [BurstCompile]
        public readonly override int GetHashCode()
        {
            return (int)math.hash(new int3(Item1.GetHashCode(), Item2.GetHashCode(), Item3.GetHashCode()));
        }
    }
}

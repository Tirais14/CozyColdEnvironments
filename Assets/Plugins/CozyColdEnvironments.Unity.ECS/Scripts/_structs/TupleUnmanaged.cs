#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public struct TupleUnmanaged<T1, T2>
        where T1 : struct
        where T2 : struct
    {
        public T1 Item1;
        public T2 Item2;
    }

    public struct TupleUnmanaged<T1, T2, T3>
        where T1 : struct
        where T2 : struct
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
    }
}

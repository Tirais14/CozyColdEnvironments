using Unity.Physics;

#nullable enable
namespace CCEnvs.UnityX.ECS.Physics
{
    public struct ClosestHitCollectorNaNSafe<T> : ICollector<T>
        where T : struct, IQueryResult
    {
        public float MaxFraction { get; private set; }

        public int NumHits { get; private set; }

        public T ClosestHit { get; private set; }

        public readonly bool EarlyOutOnFirstHit => false;
        public readonly bool IsNotCountNaN;

        public ClosestHitCollectorNaNSafe(float maxFraction, bool countNaN = true)
            :
            this()
        {
            MaxFraction = maxFraction;
            IsNotCountNaN = !countNaN;
        }

        public bool AddHit(T hit)
        {
            if (NumHits != 0 && hit.Fraction > MaxFraction)
                return false;

            ClosestHit = hit;
            MaxFraction = hit.Fraction;
            NumHits = 1;
            return true;
        }
    }
}

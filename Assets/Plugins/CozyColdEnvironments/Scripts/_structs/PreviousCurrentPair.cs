#nullable enable
namespace CCEnvs
{
    public static class PreviousCurrentPair
    {
        public static PreviousCurrentPair<T> Create<T>(T previous, T current)
        {
            return new PreviousCurrentPair<T>(previous, current);
        }
    }
    public readonly struct PreviousCurrentPair<T>
    {
        public T Previous { get; }
        public T Current { get; }

        public PreviousCurrentPair(T previous, T current)
        {
            Previous = previous;
            Current = current;
        }
    }
}

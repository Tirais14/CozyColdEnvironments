#nullable enable
namespace CCEnvs
{
    public static class PreviousCurrentPair
    {
        public static PreviousCurrentPair<T> Create<T>(T previous, T current)
        {
            return new PreviousCurrentPair<T>(previous, current);
        }

        public static PreviousCurrentPair<TPrevious, TCurrent> CreateT<TPrevious, TCurrent>(TPrevious previous, TCurrent current)
        {
            return new PreviousCurrentPair<TPrevious, TCurrent>(previous, current);
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

    public readonly struct PreviousCurrentPair<TPrevious, TCurrent>
    {
        public TPrevious Previous { get; }
        public TCurrent Current { get; }

        public PreviousCurrentPair(TPrevious previous, TCurrent current)
        {
            Previous = previous;
            Current = current;
        }
    }
}

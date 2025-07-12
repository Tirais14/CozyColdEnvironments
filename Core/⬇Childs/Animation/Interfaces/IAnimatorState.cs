namespace UTIRLib.Animation
{
    public interface IAnimatorState
    {
        public string StateName { get; }
        public int Layer { get; }
    }
}
namespace UTIRLib.Animation
{
    public readonly struct AnimatorState : IAnimatorState
    {
        private readonly string stateName;
        private readonly int layer;

        public readonly string StateName => stateName;
        public readonly int Layer => layer;

        public AnimatorState(string stateName, int layer)
        {
            this.stateName = stateName;
            this.layer = layer;
        }
    }
}
#nullable enable
namespace CCEnvs
{
    public readonly struct OperatorChain
    {
        private readonly string[] parts;

        public OperatorChain(params string[] parts)
        {
            this.parts = parts;
        }

        public static implicit operator string(OperatorChain operatorChain)
        {
            return operatorChain.ToString();
        }

        public override string ToString()
        {
            return parts.JoinStrings('.');
        }
    }
}

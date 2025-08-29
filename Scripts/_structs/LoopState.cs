#nullable enable
namespace CCEnvs
{
    public class LoopState
    {
        public LoopKeyword Value { get; set; }

        public void Reset() => Value = LoopKeyword.None;
    }
}

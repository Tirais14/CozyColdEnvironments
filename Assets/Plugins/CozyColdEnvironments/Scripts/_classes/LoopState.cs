#nullable enable
using CCEnvs.Proeprties;

namespace CCEnvs
{
    public record LoopState
    {
        public bool Break { get; set; }
        public Trigger<bool> Continue { get; } = new();
    }
}

using CCEnvs.Properties;

#nullable enable
namespace CCEnvs
{
    public record LoopState
    {
        public bool Break { get; set; }
        public ResetableProperty<bool> Continue { get; } = new();
    }
}

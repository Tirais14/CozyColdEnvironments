using CCEnvs.Reflection.Data;

#nullable enable
namespace CCEnvs.Reflection
{
    public record ConstructorBindings : MemberBindings
    {
        public ExplicitArguments Arguments { get; set; }
        public bool WithDefaultParamsAsEmptyConstuctor { get; set; }
    }
}

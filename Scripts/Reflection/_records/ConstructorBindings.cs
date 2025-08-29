using CCEnvs.Reflection.ObjectModel;

#nullable enable
namespace CCEnvs.Reflection
{
    public record ConstructorBindings : MemberBindings
    {
        public ExplicitArguments Arguments { get; set; }
    }
}

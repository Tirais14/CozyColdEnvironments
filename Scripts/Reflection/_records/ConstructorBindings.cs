using CozyColdEnvironments.Reflection.ObjectModel;

#nullable enable
namespace CozyColdEnvironments.Reflection
{
    public record ConstructorBindings : MemberBindings
    {
        public ExplicitArguments Arguments { get; set; }
    }
}

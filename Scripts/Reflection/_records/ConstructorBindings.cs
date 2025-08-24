using UTIRLib.Reflection.ObjectModel;

#nullable enable
namespace UTIRLib.Reflection
{
    public record ConstructorBindings : MemberBindings
    {
        public ExplicitArguments Arguments { get; set; }
    }
}

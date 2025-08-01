using System;
using System.Linq;

#nullable enable
namespace UTIRLib.Reflection
{
    public record ConstructorParameters : TypeMemberParameters
    {
        public InvokableArguments ArgumentsData {
            get => new(Signature, Arguments);
            set
            {
                Signature = new InvokableSignature(value.Signature.ToArray(), value.Signature.AllowInheritance);
                Arguments = value.ArgumentValues.ToArray();
            }
        }

        public InvokableSignature Signature { get; set; }
        public object?[] Arguments { get; set; } = Array.Empty<object?>();
    }
}

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
                Signature = new InvokableSignature(value.Signature.ToArray());
                Arguments = value.ArgumentValues.ToArray();
            }
        }

        public InvokableSignature Signature { get; private set; }
        public object?[] Arguments { get; private set; } = Array.Empty<object?>();
    }
}

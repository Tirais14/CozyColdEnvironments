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
                Signature = value.Signature.ToArray();
                Arguments = value.ArgumentValues.ToArray();
            }
        }

        public Type[] Signature { get; private set; } = Array.Empty<Type>();
        public object?[] Arguments { get; private set; } = Array.Empty<object?>();
    }
}

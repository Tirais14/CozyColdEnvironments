using CCEnvs.Reflection.Data;
using System;

#nullable enable
namespace CCEnvs.Reflection
{
    public record MethodBindings : MemberBindings
    {
        public ExplicitArguments Arguments { get; set; }
        public Type[] GenericArguments { get; set; } = Array.Empty<Type>();
        public bool HasGenericArguments => GenericArguments.IsNotNullOrEmpty();
    }
}

using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Reflection
{
    public record MethodBindings : ConstructorBindings
    {
        public string MethodName { get; set; } = string.Empty;
        public object? Target { get; set; }
        public Type[] GenericArguments { get; set; } = Type.EmptyTypes;
        public bool HasGenericArguments => GenericArguments.IsNotNullOrEmpty();
    }
}

using System;
using System.Reflection;

#nullable enable
namespace UTIRLib.Reflection
{
    public record TypeMemberParameters
    {
        public BindingFlags BindingFlags { get; set; } = BindingFlags.Default;
        public CallingConventions CallingConventions { get; set; } = CallingConventions.Standard;
        public ParameterModifier[] ParameterModifiers { get; set; } = Array.Empty<ParameterModifier>();
        public Binder? Binder { get; set; } = null;
    }
}

using System;
using System.Reflection;

#nullable enable
namespace UTIRLib.Reflection
{
    public record MemberBindings
    {
        public BindingFlags BindingFlags { get; set; } = BindingFlagsDefault.InstancePublic;
        public CallingConventions CallingConventions { get; set; } = CallingConventions.Standard | CallingConventions.HasThis;
        public ParameterModifier[] ParameterModifiers { get; set; } = Array.Empty<ParameterModifier>();
        public Binder? Binder { get; set; } = null;
    }
}

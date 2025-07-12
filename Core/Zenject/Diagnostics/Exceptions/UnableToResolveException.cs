using System;
using System.Reflection;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

namespace UTIRLib.Zenject
{
    public sealed class UnableToResolveException : TirLibException
    {
        private const string MESSAGE = "Unable to resolve{0}.";

        public UnableToResolveException() : base(MESSAGE, null)
        { }

        public UnableToResolveException(Type targetType) : base(MESSAGE, $" {targetType.Name}")
        { }

        public UnableToResolveException(object target) : base(MESSAGE, $" {target.GetTypeName()}")
        { }

        public UnableToResolveException(MemberInfo targetMember) :
            base(MESSAGE, $" {targetMember.GetTypeName().Delete("Info")} {targetMember.Name}")
        { }
    }
}
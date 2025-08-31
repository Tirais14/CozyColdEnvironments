#nullable enable
using CCEnvs.Reflection;
using System;

namespace CCEnvs.Diagnostics
{
    public class TypeIsNotExpectedTypeException : CCException
    {
        public TypeIsNotExpectedTypeException()
        {
        }

        public TypeIsNotExpectedTypeException(Type left, Type right)
            :
            base($"Type {left.GetName()} is not {right.GetName()}.")
        {
        }
    }
}

using System;
using static CCEnvs.Diagnostics.ExceptionMessageConstructor;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public class TypeCastException : CCException
    {
        public TypeCastException()
        {
        }

        public TypeCastException(Type? fromType, Type toType)
            :
            base($"From {fromType} to {toType}.")
        {
        }
    }
}

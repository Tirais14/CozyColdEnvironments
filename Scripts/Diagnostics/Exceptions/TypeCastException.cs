using System;

#nullable enable
namespace UTIRLib.Diagnostics
{
    public class TypeCastException : TirLibException
    {
        public TypeCastException()
        {
        }

        public TypeCastException(Type? fromType, Type toType)
            :
            base(ConstructMessage(typeof(TypeCastException), new Internal.ArgumentInfo(fromType, typeof(Type)), toType))
        {
        }
    }
}

using System;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib
{
    public class EnumNotFlagsException : TirLibException
    {
        public EnumNotFlagsException()
        {
        }

        public EnumNotFlagsException(Type type)
            : base($"Type {type.GetProccessedName()} is not enum flag.")
        {
        }

        public EnumNotFlagsException(Type type, string message)
            : base($"Type {type.GetProccessedName()} is not enum flag. " + message)
        {
        }
    }
}

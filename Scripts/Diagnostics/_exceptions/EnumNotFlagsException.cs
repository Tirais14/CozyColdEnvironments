using System;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib.Diagnostics
{
    public class EnumNotFlagsException : TirLibException
    {
        public EnumNotFlagsException()
        {
        }

        public EnumNotFlagsException(Type type)
            : base($"Type {type.GetName()} is not enum flag.")
        {
        }

        public EnumNotFlagsException(Type type, string message)
            : base($"Type {type.GetName()} is not enum flag. " + message)
        {
        }
    }
}

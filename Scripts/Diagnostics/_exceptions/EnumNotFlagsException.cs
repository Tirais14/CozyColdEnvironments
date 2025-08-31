using System;
using CCEnvs.Reflection;

#nullable enable
namespace CCEnvs.Diagnostics
{
    public class EnumNotFlagsException : CCException
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

using System;
using CozyColdEnvironments.Reflection;

#nullable enable
namespace CozyColdEnvironments.Diagnostics
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

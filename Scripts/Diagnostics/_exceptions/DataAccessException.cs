#nullable enable
using CCEnvs.Reflection;
using System;

namespace CCEnvs.Diagnostics
{
    public class DataAccessException : CCException
    {
        public DataAccessException()
        {
        }

        public DataAccessException(object? message, Exception? innerException = null) 
            :
            base(GetMessage(message), innerException)
        {
        }

        private static string GetMessage(object? value)
        {
            if (value.IsNull())
                return "Value = null";
            if (value is Type type)
                return $"Type = {type.Name}";

            if (value is string str)
                return str;

            return $"Value = {value.GetType().GetName()}";
        }
    }
}

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

        public DataAccessException(object? data, string? message = null, Exception? innerException = null) 
            :
            base($"{ConvertData(data)}. {message}", innerException)
        {
        }

        private static string ConvertData(object? value)
        {
            if (value.IsNull())
                return "Value = null";
            if (value is Type type)
                return $"Type = {type.Name}";

            if (value is string str)
                return str;

            return $"{value.GetType().GetName()} = {value}";
        }
    }
}

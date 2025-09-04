#nullable enable
using CCEnvs.Reflection;
using System;

namespace CCEnvs.Diagnostics
{
    public class CCInvalidDataException : CCException
    {
        public CCInvalidDataException()
        {
        }

        public CCInvalidDataException(object? data,
                                    string dataName = "Value",
                                    string? message = null,
                                    Exception? innerException = null)
            :
            base($"{dataName} = {ConvertData(data)}. {message}", innerException)
        {
        }

        private static string ConvertData(object? data)
        {
            if (data.IsNull())
                return "null";
            if (data is Type type)
                return type.GetName();
            if (data is string str)
                return str;

            return data.ToString();
        }
    }
}

using System;

#nullable enable

namespace UTIRLib.Diagnostics
{
    public class TirLibException : Exception
    {
        private const string PARAM_NAME_MSG = "Parameter: {0}";

        public TirLibException() : base()
        {
        }

        public TirLibException(string message) : base(message)
        {
        }

        public TirLibException(string notFormattedMessage, params object[] args) :
            base(string.Format(notFormattedMessage, args))
        {
        }

        protected static string GetParamNameMsg(string paramName) => string.Format(PARAM_NAME_MSG, paramName);

        protected static string GetObjectTypeName(object? obj) => obj.IsNull() ? "null" : obj.GetType().Name;
    }
}
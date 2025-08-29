#nullable enable

namespace CozyColdEnvironments.Diagnostics
{
    public sealed class StringArgumentException : TirLibException
    {
        public StringArgumentException()
        {
        }

        public StringArgumentException(string paramName) : base(GetParamNameMsg(paramName))
        {
        }

        public StringArgumentException(string paramName, string? value)
            : base($"String: {StringException.Resolve(value)}. {GetParamNameMsg(paramName)}")
        {
        }
    }
}
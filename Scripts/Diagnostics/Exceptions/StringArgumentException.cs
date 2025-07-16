#nullable enable

namespace UTIRLib.Diagnostics
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
            : base($"String: {WrongStringException.Resolve(value)}. {GetParamNameMsg(paramName)}")
        {
        }
    }
}
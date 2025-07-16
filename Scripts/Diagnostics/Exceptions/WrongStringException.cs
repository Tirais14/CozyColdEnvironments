#nullable enable

namespace UTIRLib.Diagnostics
{
    public sealed class WrongStringException : TirLibException
    {
        public WrongStringException()
        {
        }

        public WrongStringException(string? value) : base($"String: {Resolve(value)}.")
        {
        }

        internal static string Resolve(string? value)
        {
            if (value is null)
            {
                return "null";
            }
            else if (value == string.Empty)
            {
                return "empty";
            }
            else return value;
        }
    }
}
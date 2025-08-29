#nullable enable

namespace CCEnvs.Diagnostics
{
    public sealed class StringException : TirLibException
    {
        public StringException()
        {
        }

        public StringException(string? value) : base($"String: {Resolve(value)}.")
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
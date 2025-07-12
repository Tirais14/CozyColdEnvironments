#nullable enable

namespace UTIRLib.Diagnostics
{
    public readonly struct ArgumentInfo
    {
        public readonly string argumentName;
        public readonly object? argumentValue;

        public ArgumentInfo(object? argumentValue)
        {
            argumentName = string.Empty;
            this.argumentValue = argumentValue;
        }

        public ArgumentInfo(string? argumentName, object? argumentValue)
        {
            this.argumentName = argumentName ?? string.Empty;
            this.argumentValue = argumentValue;
        }
    }
}
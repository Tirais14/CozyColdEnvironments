using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.FileSystem.ScriptUtils
{
    public struct DefaultValueConverter : IConverter<string?>
    {
        public readonly string? Convert(object? value)
        {
            if (value is string str)
                return str;
            else 
                return value?.ToString();
        }

        public readonly bool TryConvert(object? value, [NotNullWhen(true)] out string? result)
        {
            result = Convert(value);

            return result.IsNotNullOrEmpty();
        }

        public readonly bool TryConvert(object? value, [NotNullWhen(true)] out object? result)
        {
            if (TryConvert(value, out string? str))
            {
                result = str;
                return true;
            }

            result = null;
            return false;
        }

        readonly object? IConverter.Convert(object? value) => Convert(value);
    }
}
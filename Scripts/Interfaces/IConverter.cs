#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace UTIRLib
{
    public interface IConverter
    {
        object? Convert(object? value);

        bool TryConvert(object? value, [NotNullWhen(true)] out object? result);
    }

    public interface IConverter<TOut> : IConverter
    {
        new TOut Convert(object? value);

        bool TryConvert(object? value, [NotNullWhen(true)] out TOut result);
    }

    public interface IConverter<T, TOut> : IConverter<TOut>
    {
        TOut Convert(T? value);

        bool TryConvert(T? value, [NotNullWhen(true)] out TOut result);
    }
}
#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace CozyColdEnvironments
{
    public interface ITypeConverter
    {
        object? Convert(object? value);

        bool TryConvert(object? value, [NotNullWhen(true)] out object? result);
    }

    public interface ITypeConverter<TOut> : ITypeConverter
    {
        new TOut Convert(object? value);

        bool TryConvert(object? value, [NotNullWhen(true)] out TOut result);
    }

    public interface IConverter<T, TOut> : ITypeConverter<TOut>
    {
        TOut Convert(T? value);

        bool TryConvert(T? value, [NotNullWhen(true)] out TOut result);
    }
}
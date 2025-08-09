#nullable enable

namespace UTIRLib.Patterns.Composite
{
    public interface IComposite : IReadOnlyComposite
    {
        void Add(object value);

        void AddRange(params object[] values);

        void Remove(object? value);

        void RemoveRange(params object?[] values);
    }

    public interface IComposite<T> : IComposite, IReadOnlyComposite<T>
    {
        void Add(T value);

        void AddRange(params T[] values);

        void Remove(T? value);

        void RemoveRange(params T?[] values);
    }
}
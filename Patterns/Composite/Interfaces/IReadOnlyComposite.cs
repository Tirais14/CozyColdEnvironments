#nullable enable

using System.Collections.Generic;

namespace UTIRLib.Patterns.Composite
{
    public interface IReadOnlyComposite
    {
        bool Contains(object? value);

        object GetChild(int index);
    }

    public interface IReadOnlyComposite<T> : IReadOnlyComposite, IReadOnlyList<T>
    {
        bool Contains(T? value);

        new T GetChild(int index);
    }
}
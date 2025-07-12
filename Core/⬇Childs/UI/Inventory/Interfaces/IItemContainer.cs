using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.UI
{
    public interface IItemContainer
    {
        IItem? Item { get; }
        int Quantity { get; }

        T? GetItem<T>() where T : IItem;

        bool TryGetItem<T>([NotNullWhen(true)] out T? item) where T : IItem;

        bool Contains(IItem item);
    }
}
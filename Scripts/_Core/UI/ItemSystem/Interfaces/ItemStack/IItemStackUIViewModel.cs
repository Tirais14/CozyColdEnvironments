using UniRx;
using UnityEngine;

#nullable enable
namespace UTIRLib.UI.ItemSystem
{
    public interface IItemStackUIViewModel : IViewModel<IItemStackUIReactive>
    {
        IReadOnlyReactiveProperty<Sprite?> ItemIcon { get; }
        IReadOnlyReactiveProperty<string> ItemCount { get; }
    }
}

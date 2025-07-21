using UniRx;
using UnityEngine;

#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public interface IItemStackUIViewModel : IViewModel<IItemStackUIReactive>
    {
        IReadOnlyReactiveProperty<Sprite?> ItemIcon { get; }
        IReadOnlyReactiveProperty<string> ItemCount { get; }
    }
}

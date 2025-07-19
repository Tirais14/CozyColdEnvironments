using UniRx;
using UnityEngine;

#nullable enable
namespace UTIRLib.UI
{
    public interface IItemStackUIViewModel : IViewModel, IViewModel<IItemStackUIReactive>
    {
        IReadOnlyReactiveProperty<Sprite?> ItemIcon { get; }
        IReadOnlyReactiveProperty<string> ItemCount { get; }
    }
}

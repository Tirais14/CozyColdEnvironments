using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI
{
    public interface IIconViewModel : IViewModel
    {
        ReadOnlyReactiveProperty<Sprite> IconView { get; }
    }
}

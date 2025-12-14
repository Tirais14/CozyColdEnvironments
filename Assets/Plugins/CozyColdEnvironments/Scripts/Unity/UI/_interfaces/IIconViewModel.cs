using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IIconViewModel : IViewModel
    {
        ReadOnlyReactiveProperty<Sprite> IconView { get; }
    }
}

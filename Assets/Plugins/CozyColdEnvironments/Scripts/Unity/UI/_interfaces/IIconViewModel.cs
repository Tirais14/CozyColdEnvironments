using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IIconViewModel : IViewModel
    {
        IReadOnlyReactiveProperty<Sprite> IconView { get; }
    }
}

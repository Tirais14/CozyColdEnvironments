using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface ISpriteTextViewModel<TModel> : IViewModel<TModel>
    {
        IReactiveProperty<Sprite> Icon { get; }
        IReactiveProperty<string> Text { get; }
    }
}

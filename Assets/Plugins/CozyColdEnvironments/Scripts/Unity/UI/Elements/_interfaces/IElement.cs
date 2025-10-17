#nullable enable
using UnityEngine.Events;

namespace CCEnvs.Unity.UI.Elements
{
    public interface IElement : IShowable
    {
        UnityEvent OnShowed { get; }
        UnityEvent OnHided { get; }
    }
}

using CCEnvs.Unity.Components;
using UnityEngine.Events;
using UnityEngine;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    public abstract class Element : CCBehaviour, IElement
    {
        private readonly ReactiveProperty<bool> isVisible = new();

        [field: SerializeField]
        public UnityEvent OnShowed { get; private set; } = new UnityEvent();

        [field: SerializeField]
        public UnityEvent OnHided { get; private set; } = new UnityEvent();

        public IReadOnlyReactiveProperty<bool> IsVisible => isVisible;

        public void Hide()
        {
            gameObject.SetActive(false);
            isVisible.Value = false;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            isVisible.Value = true;
        }
    }
}

using CCEnvs.Unity.Components;
using UnityEngine.Events;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    public abstract class UIElement : CCBehaviour, IUIElement
    {
        [field: SerializeField]
        public UnityEvent OnShowed { get; private set; } = new UnityEvent();

        [field: SerializeField]
        public UnityEvent OnHided { get; private set; } = new UnityEvent();

        public bool IsVisible { get; private set; }

        public void Hide()
        {
            gameObject.SetActive(false);
            IsVisible = false;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            IsVisible = true;
        }
    }
}

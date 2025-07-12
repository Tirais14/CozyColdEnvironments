using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;
using UTIRLib.Diagnostics;
using UTIRLib.Injector;

#nullable enable

namespace UTIRLib.UI
{
    public abstract class View : CanvasElement
    {
        private Image? imageInternal;

        [SuppressMessage("Style", "IDE1006:Naming Styles")]
        protected Image image {
            get {
                if (imageInternal == null)
                {
                    imageInternal = GetComponent<Image>().
                        ThrowIfNull("It's component is optional and not found, but called. It's mistake?");
                }

                return imageInternal;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            BindToViewModel();
        }

        protected void SetCanvasEnabledState(bool state) => canvas.enabled = state;

        protected void SetImageEnabledState(bool state) => image.enabled = state;

        protected abstract void BindToViewModel();
    }

    public abstract class View<TViewModel> : View
        where TViewModel : class
    {
        [SerializeField]
        [GetComponentIfNull]
        protected TViewModel viewModel = null!;
    }
}
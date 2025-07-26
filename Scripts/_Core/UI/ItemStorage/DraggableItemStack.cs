using UnityEngine;
using UnityEngine.UI;
using UTIRLib.GameSystems.Storage;

#nullable enable
#pragma warning disable IDE0044
namespace UTIRLib.UI.ItemStorage
{
    [RequireComponent(typeof(Image))]
    public class DraggableItemStack : MonoX, IMovable, IStateToggleable
    {
        private Vector2 defaultPosition;

        [GetBySelf]
        private Image image = null!;

        public Vector2 Position {
            get => transform.position;
            set => transform.position = value;
        }
        public bool IsEnabled {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }
        public IItemStack? SourceItemStack { get; set; }

        protected override void OnStart()
        {
            base.OnStart();

            defaultPosition = transform.localPosition;
        }

        public void ResetPosition()
        {
            transform.localPosition = defaultPosition;
        }
    }
}

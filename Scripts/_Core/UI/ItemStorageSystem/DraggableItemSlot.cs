using UnityEngine;
using UnityEngine.UI;
using UTIRLib.Diagnostics;
using UTIRLib.GameSystems.ItemStorageSystem;

#nullable enable
#pragma warning disable IDE0044
namespace UTIRLib.UI.ItemStorageSystem
{
    [RequireComponent(typeof(Image))]
    public class DraggableItemSlot : MonoX, IStateSwitchable
    {
        [GetBySelf]
        private Image image = null!;

        public IItemSlot? SourceSlot { get; set; }
        public bool IsEnabled { get; private set; }

        public void Disable()
        {
            image.sprite = null;
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            if (SourceSlot.IsNull())
                throw new System.Exception("Item stack not setted.");

            image.sprite = SourceSlot.Item.Icon;
            gameObject.SetActive(true);
        }
    }
}

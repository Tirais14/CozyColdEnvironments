using System;
using UnityEngine;
using UTIRLib.Diagnostics;

#nullable enable

namespace UTIRLib.UI
{
    public class StorageSlotViewModel : ViewModel<StorageSlotModel>
    {
        /// <exception cref="ArgumentNullException"></exception>
        public void DragItemStack(IItemStack itemStack)
        {
            if (itemStack.IsNull())
            {
                throw new ArgumentNullException(nameof(itemStack));
                return;
            }
            if (itemStack.IsEmpty)
            {
                Debug.LogError("Cannot not drag empty item stack.");
                return;
            }

            model.ItemStack.Put(itemStack);
        }

        protected override void BindToModel()
        { }
    }
}
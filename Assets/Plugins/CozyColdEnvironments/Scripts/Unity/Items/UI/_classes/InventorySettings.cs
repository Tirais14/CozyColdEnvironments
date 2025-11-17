using CCEnvs.Diagnostics;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    [DisallowMultipleComponent]
    public sealed class InventorySettings : ViewElementComponentCommand
    {
        [SerializeField]
        private GameObject itemContainerPrefab;

        [Min(0)]
        [SerializeField]
        private int itemContainerCount;

        protected override void OnUpdate()
        {
            if (Application.isPlaying)
            {
                if (itemContainerCount <= 0)
                    return;

                this.AppealTo()
                    .Model<IInventory>()
                    .Lax()
                    .Match(
                        some: inv =>
                        {
                            inv.ItemContainerPrefab = itemContainerPrefab;
                            inv.SetContainerCountByPrefab(itemContainerCount);
                        },
                        none: () => this.PrintError("Not found inventory.")
                    );
            }
        }
    }
}

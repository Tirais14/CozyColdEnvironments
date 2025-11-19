using CCEnvs.Diagnostics;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    [DisallowMultipleComponent]
    public sealed class InventorySettings : ViewComponentCommand
    {
        [SerializeField]
        private GameObject itemContainerPrefab;

        [Min(0)]
        [SerializeField]
        private int itemContainerCount;

        protected override void Start()
        {
            base.Start();

            PreUpdateAction(() =>
            {
                this.QueryTo()
                    .Model<IInventory>()
                    .Lax()
                    .Match(
                        some: inv =>
                        {
                            inv.NodePrefab = itemContainerPrefab;
                            inv.SetNodeCountByPrefab(itemContainerCount);
                        },
                        none: () => this.PrintError("Not found inventory.")
                        );
            });
        }
    }
}

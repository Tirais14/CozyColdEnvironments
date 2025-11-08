using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Storages;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public sealed class SetItemContainerCount : CCBehaviourTemporary
    {
        [SerializeField]
        private GameObject itemContainerPrefab;

        [Min(0)]
        [SerializeField]
        private int count;

        protected override void OnUpdate()
        {
            if (itemContainerPrefab == null)
            {
                this.PrintError("Item container prefab not found.");
                return;
            }
            if (count <= 0)
                return;

            this.FindFor().Model<IInventory>().Lax().Match(
                some: inv => inv.SetContainerCount(count, itemContainerPrefab),
                none: () => this.PrintError("Not found inventory model.")
                );
        }
    }
}

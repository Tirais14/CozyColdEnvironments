using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Storages;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public sealed class SetItemContainerCount : ViewElementComponentCommand
    {
        [SerializeField]
        private GameObject prefab;

        [Min(0)]
        [SerializeField]
        private int count;

        protected override void OnUpdate()
        {
            if (prefab == null)
            {
                this.PrintError("Item container prefab not found.");
                return;
            }
            if (count <= 0)
                return;

            this.FindFor().Model<IInventory>().Lax().Match(
                some: inv => inv.SetContainerCount(count, prefab),
                none: () => this.PrintError("Not found inventory model.")
                );
        }
    }
}

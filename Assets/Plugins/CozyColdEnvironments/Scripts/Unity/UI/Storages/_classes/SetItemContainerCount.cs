using CCEnvs.Diagnostics;
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
            if (Application.isPlaying)
            {

                if (prefab == null)
                {
                    this.PrintError("Item container prefab not found.");
                    return;
                }
                if (count <= 0)
                    return;

                this.Appeal().Model<IInventory>().Lax().Match(
                    some: inv => inv.SetContainerCount(count, prefab),
                    none: () => this.PrintError("Not found inventory model.")
                    );
            }
        }

//#if UNITY_EDITOR
//        private void OnValidate()
//        {
//            if (!Application.isPlaying)
//            {
//                if (prefab == null)
//                {
//                    this.PrintError("Not found prefab");
//                    return;
//                }
//                var bag = this.AskFor().ByChildren().Component<GameObjectBag>().Raw;
//                if (bag == null)
//                {
//                    this.PrintError($"Not found {nameof(GameObjectBag)}".Humanize());
//                    return;
//                }

//                foreach (var go in bag.AskFor().ExcludeSelf().ChildrenGameObjects())
//                    DestroyImmediate(go);

//                for (int i = 0; i < count; i++)
//                    Instantiate(prefab).transform.SetParent(bag.transform);
//            }
//        }
//#endif
    }
}

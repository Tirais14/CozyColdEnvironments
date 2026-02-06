#nullable enable
using CCEnvs.Unity.Injections;
using UnityEngine;

#pragma warning disable S2933
namespace CCEnvs.Unity.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MaterializedItemContainer2D : AMaterializedItemContainer
    {
        [GetBySelf]
        new protected SpriteRenderer renderer = null!;

        protected override void OnSetItemContainer()
        {
            renderer.sprite = itemContainer.Item.GetValueUnsafe().Icon;
        }
    }
}

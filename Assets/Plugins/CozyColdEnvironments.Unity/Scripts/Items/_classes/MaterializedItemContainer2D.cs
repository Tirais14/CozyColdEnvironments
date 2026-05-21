#nullable enable
using CCEnvs.UnityX.Injections;
using UnityEngine;

#pragma warning disable S2933
namespace CCEnvs.UnityX.Items
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

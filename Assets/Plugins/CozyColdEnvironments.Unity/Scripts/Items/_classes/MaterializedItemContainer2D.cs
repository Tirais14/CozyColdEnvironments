#nullable enable
using CCEnvs.FuncLanguage;
using CCEnvs.UnityX.ComponentInjections;
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
            renderer.sprite = itemContainer.Item.Maybe().Map(item => item.Icon).GetValue();
        }
    }
}

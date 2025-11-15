#nullable enable
using CCEnvs.Unity.Injections;
using System;
using UnityEngine;

#pragma warning disable S2933
namespace CCEnvs.Unity.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MaterializedItemContainer2D : AMaterializedItemContainer
    {
        [GetBySelf]
        new protected SpriteRenderer renderer = null!;

        public static MaterializedItemContainer2D Create(IItemContainer itemContainer,
            Vector2 position,
            Action<MaterializedItemContainer2D>? callback = null)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));
            if (itemContainer.Item.IsNone)
                CC.Throw.ArgumentException("none", nameof(itemContainer));

            var go = new GameObject(nameof(ItemContainer),
                typeof(SpriteRenderer),
                typeof(MaterializedItemContainer2D)
                );

            go.transform.position = position;
            var created = go.AppealTo().Component<MaterializedItemContainer2D>().Strict();
            callback?.Invoke(created);

            return created;
        }

        protected override void OnSetItemContainer()
        {
            renderer.sprite = itemContainer.Item.GetValueUnsafe().Icon;
        }
    }
}

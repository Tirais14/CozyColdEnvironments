using CCEnvs.Unity.Injections;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MaterializedItem2D : AMaterializedItem
    {
        [field: GetBySelf]
        protected SpriteRenderer renderer { get; private set; } = null!;

        public static MaterializedItem2D Create(IItem item, Vector3 position, Action<MaterializedItem2D>? callback = null)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            var go = new GameObject(item.Name, 
                typeof(SpriteRenderer),
                typeof(MaterializedItem2D)
                );

            go.transform.position = position;
            var created = go.AppealTo().Component<MaterializedItem2D>().Strict();
            callback?.Invoke(created);

            return created;
        }

        protected override void OnSetInternalItem()
        {
            renderer.sprite = item.Icon;
        }
    }
}

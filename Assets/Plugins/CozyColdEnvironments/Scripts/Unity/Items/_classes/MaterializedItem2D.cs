using CCEnvs.Unity.Injections;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class MaterializedItem2D : AMaterializedItem
    {
        [field: GetBySelf]
        new protected SpriteRenderer renderer { get; private set; } = null!;

        protected override void OnSetInternalItem()
        {
            renderer.sprite = item.Icon;
        }
    }
}

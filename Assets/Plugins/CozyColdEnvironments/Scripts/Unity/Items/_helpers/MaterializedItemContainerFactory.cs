using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public static class MaterializedItemContainerFactory
    {
        public static AMaterializedItemContainer Create(IItemContainer itemContainer,
            Vector3 position,
            RenderMode renderMode,
            Action<AMaterializedItemContainer>? callback = null)
        {
            CC.Guard.IsNotNull(itemContainer, nameof(itemContainer));

            return renderMode switch
            {
                RenderMode._2D => MaterializedItemContainer2D.Create(itemContainer, (Vector2)position, callback),
                RenderMode._3D => throw new NotImplementedException(),
                _ => CC.Throw.InvalidOperation(renderMode, nameof(renderMode)).As<AMaterializedItemContainer>(),
            };
        }
    }
}

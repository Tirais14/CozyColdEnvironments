using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public static class MaterailizedItemFactory
    {
        public static AMaterializedItem Create(IItem item,
            Vector3 position,
            RenderMode renderMode,
            Action<AMaterializedItem>? callBack = null)
        {
            CC.Guard.IsNotNull(item, nameof(item));
            if (renderMode == RenderMode.None)
                throw new ArgumentException(nameof(renderMode));

            return renderMode switch
            {
                RenderMode._2D => MaterializedItem2D.Create(item, position, callBack),
                RenderMode._3D => throw new NotImplementedException(),
                _ => throw new InvalidOperationException(renderMode.ToString()),
            };
        }
    }
}

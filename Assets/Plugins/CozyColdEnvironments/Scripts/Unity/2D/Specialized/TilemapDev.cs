using CCEnvs.Unity.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Specialized
{
    /// <summary>
    /// Destroys <see cref="Tilemap"/> <see cref="Component"/>s after start, but save parent <see cref="GameObject"/>
    /// </summary>
    public class TilemapDev : CCBehaviour
    {
        protected override void Start()
        {
            base.Start();

            if (TryGetComponent<TilemapRenderer>(out var r))
                Destroy(r);

            if (TryGetComponent<Tilemap>(out var map))
                Destroy(map);
        }
    }
}

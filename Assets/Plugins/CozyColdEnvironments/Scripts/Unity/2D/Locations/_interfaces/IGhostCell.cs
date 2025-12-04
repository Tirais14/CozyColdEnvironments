using System;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public interface IGhostCell : IDisposable
    {
        void SetPosition(Vector3Int pos);

        void Materialize(Tilemap? otherTilemap = null);
    }
}

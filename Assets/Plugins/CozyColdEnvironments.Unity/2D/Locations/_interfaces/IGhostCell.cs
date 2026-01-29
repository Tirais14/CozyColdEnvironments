using System;
using R3;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public interface IGhostCell : IDisposable
    {
        void SetPosition(Vector3Int pos);

        MaterializedCellInfo Materialize(Tilemap? otherTilemap = null);

        void ResetPosition();

        Observable<MaterializedCellInfo> ObserveMaterialize();

        Observable<GameObject> ObserveGhostGameObjectInstantiated();
    }
}

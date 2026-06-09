using CCEnvs.Serialization;
using System;
using Unity.Rendering;

#nullable enable
namespace CCEnvs.UnityX.ECS.Rendering
{
    [Serializable]
    public struct MaterialMeshIndexDTO : IDataTransferObject<MaterialMeshIndex>
    {
        public int MaterialIndex;

        public int MeshIndex;

        public int SubMeshIndex;

        public static implicit operator MaterialMeshIndex(MaterialMeshIndexDTO instance)
        {
            return instance.Materialize();
        }

        public MaterialMeshIndex Materialize()
        {
            return new MaterialMeshIndex
            {
                MaterialIndex = MaterialIndex,
                MeshIndex = MeshIndex,
                SubMeshIndex = SubMeshIndex
            };
        }
    }
}

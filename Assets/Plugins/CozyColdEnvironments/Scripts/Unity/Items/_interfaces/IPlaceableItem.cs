using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Items
{
    public interface IPlaceableItem : IItem
    {
        GameObject WorldPrefab { get; }
    }
}

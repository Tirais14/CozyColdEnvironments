#nullable enable
using UnityEngine;

namespace CCEnvs.UnityX.Items
{
    public interface IPlaceableItem : IItem
    {
        Object WorldObject { get; }
    }

    public interface IPlaceableItem<T> : IPlaceableItem
        where T : UnityEngine.Object
    {
        new T WorldObject { get; }

        Object IPlaceableItem.WorldObject => WorldObject;
    }
}

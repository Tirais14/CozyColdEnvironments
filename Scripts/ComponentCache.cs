using UnityEngine;

#nullable enable
namespace UTIRLib
{
    public class ComponentCache
    {
        public readonly Transform transform;
        public readonly GameObject gameObject;

        public ComponentCache(Transform transform, GameObject gameObject)
        {
            this.transform = transform;
            this.gameObject = gameObject;
        }
    }
}

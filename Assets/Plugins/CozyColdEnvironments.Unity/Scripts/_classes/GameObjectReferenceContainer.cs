using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX
{
    public sealed class GameObjectReferenceContainer : IGameObjectReferenceContainer
    {
        public GameObject gameObject { get; }

        public GameObjectReferenceContainer(GameObject gameObject)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));
            this.gameObject = gameObject;
        }
    }
}

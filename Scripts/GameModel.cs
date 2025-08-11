using System;
using UnityEngine;
using UTIRLib.Diagnostics;

#nullable enable
#pragma warning disable IDE1006
namespace UTIRLib.Unity
{
    [Serializable]
    [DisallowMultipleComponent]
    public class GameModel : MonoX
    {
        [field: SerializeField]
        new public GameObject gameObject { get; private set; } = null!;
        new public Transform transform { get; private set; } = null!;
        public Transform Anchor { get; private set; } = null!;
        public GameModelBody Body { get; private set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            gameObject = base.gameObject;
            transform = base.transform;
        }

        protected override void OnStart()
        {
            base.OnStart();

            Anchor = transform.Find("Anchor");
            Body = GetComponentInChildren<GameModelBody>();

            if (Body == null)
                throw new ObjectNotFoundException(typeof(GameModelBody));
        }

        public static GameModel Create(string name, params Type[] components)
        {
            var gameObject = new GameObject(name, components);
            GameModel gameModel = gameObject.AddComponent<GameModel>();

            gameModel.Anchor = new GameObject("Anchor").transform;
            gameModel.Anchor.parent = gameModel.transform;
            gameModel.Anchor.tag = TirLib.Tags.TRANSFORM_OVERRIDE;

            return gameModel;
        }

        public static GameModel Create(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<GameModel>(out var gameModel))
                throw new ArgumentException($"Cannot find {nameof(GameModel)}.");

            return gameModel;
        }
    }
}

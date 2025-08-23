using System;
using UnityEngine;
using UTIRLib.Diagnostics;

#nullable enable
#pragma warning disable IDE1006
namespace UTIRLib.Unity
{
    public class GameModel : MonoX
    {
        public Transform Anchor { get; private set; } = null!;
        public GameModelBody Body { get; private set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            Anchor = transform.Find("Anchor");
            Body = GetComponentInChildren<GameModelBody>();

            if (Body == null)
                throw new ObjectNotFoundException(typeof(GameModelBody));

            if (Anchor == null)
                Anchor = transform;
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

        /// <summary>
        /// Replaces gameObject pinned to <see cref="GameModelBody"/>
        /// </summary>
        /// <param name="modelBody"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ReplaceModelBody(GameModelBody modelBody)
        {
            if (modelBody == null)
                throw new ArgumentNullException(nameof(modelBody));

            Destroy(Body.gameObject);
            Body = modelBody;

            modelBody.gameObject.transform.parent = Anchor;
        }
    }
}

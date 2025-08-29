using System;
using UnityEngine;
using CCEnvs.Diagnostics;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity
{
    public class GameModel : MonoCC
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
            gameModel.Anchor.tag = CC.Tags.TRANSFORM_OVERRIDE;

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
        /// <param name="body"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetBody(GameModelBody body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            Transform bodyTransform = body.BaseCache.transform;

            Destroy(Body.gameObject);
            Body = body;
            bodyTransform.parent = Anchor;
            bodyTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
        }
    }
}

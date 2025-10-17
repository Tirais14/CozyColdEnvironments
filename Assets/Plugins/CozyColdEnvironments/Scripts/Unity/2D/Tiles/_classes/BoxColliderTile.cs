using CCEnvs.Unity.Components;
using UnityEngine;
using UnityEngine.Tilemaps;
using ZLinq;

#nullable enable
#pragma warning disable S2696
#pragma warning disable S2325
namespace CCEnvs.Unity._2D.Tiles
{
    public sealed class BoxColliderTile : CCBehaviour
    {
        public const string SPRITE_NAME = "boxColliderTile";

        private static ulong instanceCount;
        private static Sprite boxColliderSprite = null!;

        protected override void Awake()
        {
            base.Awake();
            instanceCount++;
        }

        protected override void Start()
        {
            base.Start();

            if (instanceCount == 0)
                HideTileSprite();
        }

        private void OnDestroy()
        {
            if (instanceCount == 0)
                throw new System.InvalidOperationException($"{nameof(instanceCount)} cannot be smaller than 0.");

            instanceCount--;

            if (instanceCount == 0)
                ShowTileSprite();
        }

        private Tile GetBoxColliderTile()
        {
            return (from col in Physics2D.OverlapBoxAll(transform.position, Vector2.one * 32, 0f).ZL()
                    select col.GetComponent<Tilemap>() into map
                    where map != null
                    select map.GetTile(Vector3Int.FloorToInt(transform.position)))
                    .First()
                    .As<Tile>();
        }

        private void HideTileSprite()
        {
            var tile = GetBoxColliderTile();
            boxColliderSprite = tile.sprite;
            tile.sprite = null;
        }

        private void ShowTileSprite()
        {
            CC.Guard.NullArgument(boxColliderSprite, nameof(boxColliderSprite));

            var tile = GetBoxColliderTile();
            tile.sprite = boxColliderSprite;
        }
    }
}

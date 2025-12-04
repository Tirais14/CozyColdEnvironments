using CCEnvs.FuncLanguage;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D.Locations
{
    public static class TileHelper
    {
        public static TileData GetTileData(TileBase source, Vector3Int pos, ITilemap tilemap)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            TileData tileData = default;
            source.GetTileData(pos, tilemap, ref tileData);
            return tileData;
        }

        public static Maybe<Sprite> GetTileSprite(this TileBase? source)
        {
            if (source == null)
                return Maybe<Sprite>.None;

            return source.As<Tile>().Map(tile => tile.sprite);
        }

        public static Maybe<GameObject> GetTileGameObject(this TileBase? source)
        {
            if (source == null)
                return Maybe<GameObject>.None;

            return source.As<Tile>().Map(tile => tile.gameObject);
        }
    }
}

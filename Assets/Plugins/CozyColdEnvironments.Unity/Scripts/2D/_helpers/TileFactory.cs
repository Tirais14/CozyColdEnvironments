using CCEnvs.Reflection;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.Unity._2D
{
    public static class TileFactory
    {
        public static TileBase Create(Sprite sprite, Type? tileType)
        {
            CC.Guard.IsNotNull(sprite, nameof(sprite));

            var t = ScriptableObject.CreateInstance(tileType ?? typeof(Tile));

            t.Reflect()
                .WithName(nameof(Tile.sprite))
                .Property()
                .Strict()
                .SetValue(t, sprite);

            return t.To<TileBase>();
        }
    }
}

using CCEnvs.Reflection;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable
namespace CCEnvs.U2D
{
    public static class TileFactory
    {
        public static TileBase Create(Sprite sprite, Type? tileType)
        {
            CC.Guard.IsNotNull(sprite, nameof(sprite));

            var t = ScriptableObject.CreateInstance(tileType ?? typeof(Tile));

            t.ReflectQuery()
                .Name(nameof(Tile.sprite))
                .Property()
                .Strict()
                .SetValue(t, sprite);

            return t.As<TileBase>();
        }
    }
}

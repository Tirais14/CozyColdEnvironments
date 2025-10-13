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
            CC.Guard.NullArgument(sprite, nameof(sprite));

            var t = ScriptableObject.CreateInstance(tileType ?? typeof(Tile));

            t.AsReflected().Property<Sprite>(nameof(Tile.sprite)).SetValue(sprite);

            return t.As<TileBase>();
        }
    }
}

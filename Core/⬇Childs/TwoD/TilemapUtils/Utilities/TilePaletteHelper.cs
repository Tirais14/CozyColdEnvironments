using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UTIRLib.TwoD
{
    public static class TilePaletteHelper
    {
        public static Tile[][] GetTiles(params Tilemap[] tilemaps)
        {
            Tile tileBase;
            Tilemap tilemap;
            var tiles = new List<Tile>[tilemaps.Length];
            Array.Fill(tiles, new List<Tile>());
            for (int i = 0; i < tilemaps.Length; i++)
            {
                tilemap = tilemaps[i];
                tilemap.CompressBounds();
                foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
                {
                    tileBase = tilemap.GetTile<Tile>(pos);
                    if (tileBase != null && !tiles[i].Contains(tileBase))
                    {
                        tiles[i].Add(tileBase);
                    }
                }
            }
            Tile[][] convertedTiles = new Tile[tilemaps.Length][];
            for (int i = 0; i < convertedTiles.Length; i++)
            {
                convertedTiles[i] = tiles[i].ToArray();
            }

            return convertedTiles;
        }
    }
}
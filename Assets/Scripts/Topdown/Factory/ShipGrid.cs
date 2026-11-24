using System.Collections.Generic;
using UnityEngine;

namespace Bas.Thoth.Topdown.Factory
{
    public class ShipGrid
    {
        private readonly Dictionary<long, ShipTile> _tiles = new();

        public float TileSize { get; set; } = FactoryCatalog.TileSize;

        public IEnumerable<KeyValuePair<long, ShipTile>> Tiles => _tiles;

        public static long Key(int x, int y) => ((long)(uint)x << 32) | (uint)y;

        public static void FromKey(long key, out int x, out int y)
        {
            x = (int)(key >> 32);
            y = (int)key;
        }

        public bool TryGet(int x, int y, out ShipTile tile) => _tiles.TryGetValue(Key(x, y), out tile);

        public ShipTile Get(int x, int y) => _tiles.TryGetValue(Key(x, y), out ShipTile tile) ? tile : ShipTile.Empty;

        public void Set(int x, int y, ShipTile tile)
        {
            long key = Key(x, y);
            if (!tile.HasHull && !tile.HasBuilding)
            {
                _tiles.Remove(key);
                return;
            }

            _tiles[key] = tile;
        }

        public bool HasHull(int x, int y) => Get(x, y).HasHull;

        public bool HasBuilding(int x, int y) => Get(x, y).HasBuilding;

        public void WorldToTile(float worldX, float worldY, float shipX, float shipY, out int tileX, out int tileY)
        {
            float localX = worldX - shipX;
            float localY = worldY - shipY;
            tileX = Mathf.FloorToInt(localX / TileSize + 0.5f);
            tileY = Mathf.FloorToInt(localY / TileSize + 0.5f);
        }

        public void TileToWorld(int tileX, int tileY, float shipX, float shipY, out float worldX, out float worldY)
        {
            worldX = shipX + tileX * TileSize;
            worldY = shipY + tileY * TileSize;
        }

        public bool IsAdjacentToHull(int x, int y)
        {
            return HasHull(x + 1, y) || HasHull(x - 1, y) || HasHull(x, y + 1) || HasHull(x, y - 1);
        }

        public void Clear() => _tiles.Clear();
    }
}

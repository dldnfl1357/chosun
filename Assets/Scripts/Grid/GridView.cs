using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roller.Grid
{
    [RequireComponent(typeof(GridMap))]
    public class GridView : MonoBehaviour
    {
        public Tilemap Tilemap;
        public TileBase GrassTile;
        public TileBase ForestTile;
        public TileBase WaterTile;
        public TileBase WallTile;

        GridMap _map;

        void Start()
        {
            _map = GetComponent<GridMap>();
            if (_map == null || _map.Cells == null)
            {
                Debug.LogError("[GridView] GridMap not initialized.");
                return;
            }
            Render();
        }

        public void Render()
        {
            if (Tilemap == null)
            {
                Debug.LogError("[GridView] Tilemap reference not set.");
                return;
            }

            Tilemap.ClearAllTiles();
            for (int y = 0; y < _map.Config.Height; y++)
            {
                for (int x = 0; x < _map.Config.Width; x++)
                {
                    var cell = _map.Cells[x, y];
                    Tilemap.SetTile(new Vector3Int(x, y, 0), TileFor(cell.Terrain));
                }
            }
        }

        TileBase TileFor(TerrainType t)
        {
            switch (t)
            {
                case TerrainType.Grass:  return GrassTile;
                case TerrainType.Forest: return ForestTile;
                case TerrainType.Water:  return WaterTile;
                case TerrainType.Wall:   return WallTile;
                default: return GrassTile;
            }
        }
    }
}

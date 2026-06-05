using UnityEngine;
using Roller.Core;

namespace Roller.Grid
{
    public class GridMap : MonoBehaviour
    {
        public GridConfig Config;
        public GridCell[,] Cells { get; private set; }

        void Awake()
        {
            BuildFromConfig();
        }

        public void BuildFromConfig()
        {
            if (Config == null)
            {
                Debug.LogError("[GridMap] Config is null. Assign a GridConfig asset in the Inspector.");
                return;
            }

            Cells = new GridCell[Config.Width, Config.Height];
            bool hasInitialTerrain = Config.InitialTerrain != null && Config.InitialTerrain.Length > 0;

            for (int y = 0; y < Config.Height; y++)
            {
                for (int x = 0; x < Config.Width; x++)
                {
                    int idx = y * Config.Width + x;
                    TerrainType terrain = TerrainType.Grass;
                    if (hasInitialTerrain && idx < Config.InitialTerrain.Length)
                        terrain = Config.InitialTerrain[idx];

                    Cells[x, y] = new GridCell
                    {
                        Position = new Coord(x, y),
                        Terrain = terrain
                    };
                }
            }
        }

        public bool InBounds(Coord c) =>
            c.X >= 0 && c.Y >= 0 && c.X < Config.Width && c.Y < Config.Height;

        public GridCell Get(Coord c) =>
            InBounds(c) ? Cells[c.X, c.Y] : null;

        public Vector3 CellToWorld(Coord c) =>
            new Vector3(c.X * Config.CellSize, c.Y * Config.CellSize, 0f);

        public Coord WorldToCell(Vector3 world) =>
            new Coord(
                Mathf.RoundToInt(world.x / Config.CellSize),
                Mathf.RoundToInt(world.y / Config.CellSize));
    }
}

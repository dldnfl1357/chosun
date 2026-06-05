using Roller.Core;

namespace Roller.Grid
{
    public class GridCell
    {
        public Coord Position;
        public TerrainType Terrain;
        public IGridOccupant Occupant;

        public bool IsPassable => Terrain.IsPassable() && Occupant == null;
        public int MoveCost => Terrain.MoveCost();
    }
}

namespace Roller.Grid
{
    public enum TerrainType
    {
        Grass = 0,
        Forest = 1,
        Water = 2,
        Wall = 3
    }

    public static class TerrainExtensions
    {
        public static bool IsPassable(this TerrainType t) =>
            t == TerrainType.Grass || t == TerrainType.Forest;

        public static int MoveCost(this TerrainType t)
        {
            switch (t)
            {
                case TerrainType.Grass:  return 10;
                case TerrainType.Forest: return 15;
                default: return int.MaxValue;
            }
        }
    }
}

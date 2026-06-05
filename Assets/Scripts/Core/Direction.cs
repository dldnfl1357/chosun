using System.Collections.Generic;

namespace Roller.Core
{
    public enum Direction
    {
        N, NE, E, SE, S, SW, W, NW
    }

    public static class Directions
    {
        public static readonly Coord[] Offsets = new[]
        {
            new Coord( 0,  1), // N
            new Coord( 1,  1), // NE
            new Coord( 1,  0), // E
            new Coord( 1, -1), // SE
            new Coord( 0, -1), // S
            new Coord(-1, -1), // SW
            new Coord(-1,  0), // W
            new Coord(-1,  1), // NW
        };

        public static IEnumerable<Coord> Neighbors(Coord origin)
        {
            foreach (var offset in Offsets)
                yield return origin + offset;
        }
    }
}

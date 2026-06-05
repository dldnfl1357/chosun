using System;

namespace Roller.Core
{
    [Serializable]
    public struct Coord : IEquatable<Coord>
    {
        public int X;
        public int Y;

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coord Zero => new Coord(0, 0);

        public static Coord operator +(Coord a, Coord b) => new Coord(a.X + b.X, a.Y + b.Y);
        public static Coord operator -(Coord a, Coord b) => new Coord(a.X - b.X, a.Y - b.Y);
        public static bool operator ==(Coord a, Coord b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Coord a, Coord b) => !(a == b);

        public int ChebyshevTo(Coord other) =>
            Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));

        public int ManhattanTo(Coord other) =>
            Math.Abs(X - other.X) + Math.Abs(Y - other.Y);

        public bool Equals(Coord other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Coord c && Equals(c);
        public override int GetHashCode() => (X << 16) ^ (Y & 0xFFFF);
        public override string ToString() => $"({X},{Y})";
    }
}

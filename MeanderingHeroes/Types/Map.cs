using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types
{
    public record Location(float X, float Y)
    {
        public static implicit operator Vector2(Location location) => new Vector2(location.X, location.Y);
        public static implicit operator Location(Vector2 vector) => new Location(vector.X, vector.Y);
    }

    public readonly record struct Point(double X, double Y);

    // Implementing Hexes from the mind-blowing blog: https://www.redblobgames.com/grids/hexagons/
    // Most of this boilerplate is copy-pasta from the samples provided there.
    // Doing it this way (rather than a NuGet package) because I want to extend Hexes to hold more state
    public readonly record struct Hex
    {
        public int Q { get; init; }
        public int R { get; init; }
        public int S { get; init; }

        public Hex(int q, int r, int s)
        {
            if(q + r + s != 0) throw new ArgumentException("q + r + s must be 0");

            Q = q;
            R = r;
            S = s;
        }

        public int Length()
        {
            return (int)((Math.Abs(Q) + Math.Abs(R) + Math.Abs(S)) / 2);
        }


        public int Distance(Hex b)
        {
            return (this - b).Length();
        }

        public Hex RotateLeft() => new Hex(-S, -Q, -R);
        public Hex RotateRight() => new Hex(-R, -S, -Q);
        public Hex Neighbour(int direction) => this + Hex.Directions[direction];
        public Hex DiagonalNeighbour(int direction) => this + Hex.Diagonals[direction];

        public static Hex operator +(Hex a, Hex b) => new Hex(a.Q + b.Q, a.R + b.R, a.S + b.S);
        public static Hex operator -(Hex a, Hex b) => new Hex(a.Q - b.Q, a.R - b.R, a.S - b.S);
        public static Hex operator *(Hex a, int scale) => new Hex(a.Q * scale, a.R * scale, a.S * scale);
        public static implicit operator Hex((int, int, int) tuple) => new Hex(tuple.Item1, tuple.Item2, tuple.Item3);

        public static ImmutableArray<Hex> Directions = [
            new Hex(1, 0, -1),
            new Hex(1, -1, 0),
            new Hex(0, -1, 1),
            new Hex(-1, 0, 1),
            new Hex(-1, 1, 0),
            new Hex(0, 1, -1)
        ];

        static public ImmutableArray<Hex> Diagonals = [
            new Hex(2, -1, -1), 
            new Hex(1, -2, 1), 
            new Hex(-1, -1, 2), 
            new Hex(-2, 1, 1), 
            new Hex(-1, 2, -1), 
            new Hex(1, 1, -2)
        ];
    }
}

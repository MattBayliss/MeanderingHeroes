using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record Terrain(string Name, float MovementCost);
    public record LandTerrain : Terrain
    {
        public LandTerrain(string Name, float MovementCost) : base(Name, MovementCost) { }
    }
    public record WaterTerrain : Terrain
    {
        public WaterTerrain(string Name, float MovementCost) : base(Name, MovementCost) { }
    }

    /// <summary>
    /// A HexGrid for holding all the entities on the map.
    /// </summary>
    /// <param name="Width">How many hex columns</param>
    /// <param name="Height">How many hex rows</param>
    public record Grid
    {
        // trialling spatial partition https://gameprogrammingpatterns.com/spatial-partition.html
        // Starting with hexes being pretty big, and all entities within being able to interact,
        // and special cases with neighbour hexes too. If hexes get smaller, might use quadtrees,
        // or hextrees rather.
        public ImmutableDictionary<Hex, Terrain> Terrain { get; init; }

        public Grid(IEnumerable<(Hex Hex, Terrain Terrain)> terrainForHex)
        {
            // allow any duplicate keys to throw an error here
            Terrain = terrainForHex.ToImmutableDictionary(ht => ht.Hex, ht => ht.Terrain);
        }
        public Option<Terrain> TerrainForHex(Hex hex) => Terrain.Lookup(hex);
        public bool InBounds(Hex hex) => Terrain.ContainsKey(hex);
    }
    /// <summary>
    /// Points are standard X, Y cartesian coordinates, that map to Hex Q, R "pointy top - odd-r"
    /// Scale will be (x:10, y:0) => (q:1, r:0)
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    public record Point(float X, float Y)
    {
        public static implicit operator Vector2(Point location) => new Vector2((float)location.X, (float)location.Y);
        public static implicit operator Point(Vector2 vector) => new(vector.X, vector.Y);
        public bool Equals(Point other, float margin)
        {
            Assert.True(margin > 0);
            return MathF.Abs(X - other.X) + MathF.Abs(Y - other.Y) < margin;
        }
    }

    // Implementing Hexes from the mind-blowing blog: https://www.redblobgames.com/grids/hexagons/
    // Most of this boilerplate is copy-pasta from the samples provided there.
    // Doing it this way (rather than a NuGet package) because I want to extend Hexes to hold more state
    public readonly record struct Hex
    {
        /// <summary>
        /// Q equivalent to column
        /// </summary>
        public int Q { get; init; }
        /// <summary>
        /// R equivalent to row
        /// </summary>
        public int R { get; init; }
        /// <summary>
        /// A computed axis of `Q + R + S = 0` i.e. `S = -Q - R`
        /// </summary>
        public int S { get; init; }

        public Hex(int q, int r) : this(q, r, -q - r) { }

        internal Hex(int q, int r, int s)
        {
            Q = q;
            R = r;
            S = s;
        }

        public int Length()
        {
            return (Math.Abs(Q) + Math.Abs(R) + Math.Abs(S)) / 2;
        }

        public int Distance(Hex b)
        {
            return (this - b).Length();
        }

        public static implicit operator Hex((int, int) tuple) => new Hex(tuple.Item1, tuple.Item2);
        public static implicit operator (int Q, int R)(Hex hex) => (hex.Q, hex.R);

        public Hex RotateLeft() => new Hex(-S, -Q, -R);
        public Hex RotateRight() => new Hex(-R, -S, -Q);
        public Hex Neighbour(int direction) => this + Directions[direction];
        public Hex DiagonalNeighbour(int direction) => this + Diagonals[direction];

        public static Hex operator +(Hex a, Hex b) => new Hex(a.Q + b.Q, a.R + b.R, a.S + b.S);
        public static Hex operator -(Hex a, Hex b) => new Hex(a.Q - b.Q, a.R - b.R, a.S - b.S);
        public static Hex operator *(Hex a, int scale) => new Hex(a.Q * scale, a.R * scale, a.S * scale);

        public readonly static Hex[] Directions = [
            new Hex(1, 0, -1),
            new Hex(1, -1, 0),
            new Hex(0, -1, 1),
            new Hex(-1, 0, 1),
            new Hex(-1, 1, 0),
            new Hex(0, 1, -1)
        ];

        public readonly static Hex[] Diagonals = [
            new Hex(2, -1, -1),
            new Hex(1, -2, 1),
            new Hex(-1, -1, 2),
            new Hex(-2, 1, 1),
            new Hex(-1, 2, -1),
            new Hex(1, 1, -2)
        ];
    }
}

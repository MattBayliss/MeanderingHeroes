using LaYumba.Functional;
using System.Numerics;
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
    public readonly record struct FractionalHex
    {
        public float Q { get; init; }
        public float R { get; init; }
        public float S { get; init; }
        public FractionalHex(float q, float r)
        {
            Q = q;
            R = r;
            S = -q - r;
        }
        public FractionalHex(float q, float r, float s)
        {
            Q = q;
            R = r;
            S = s;
            if (Math.Round(q + r + s) != 0) throw new ArgumentException("Q + R + S must be 0");
        }
        public static implicit operator FractionalHex(Hex hex) => new FractionalHex(hex.Q, hex.R);
        public static explicit operator Vector3(FractionalHex fhex) => new Vector3(fhex.Q, fhex.R, fhex.S);
        public static explicit operator Vector2(FractionalHex fhex) => new Vector2(fhex.Q, fhex.R);
        public static explicit operator FractionalHex(Vector3 axialVector) => new FractionalHex(axialVector.X, axialVector.Y, axialVector.Z);
        public static explicit operator FractionalHex(Vector2 axialVector) => new FractionalHex(axialVector.X, axialVector.Y);
        public Hex Round()
        {
            int qi = (int)Math.Round(Q);
            int ri = (int)Math.Round(R);
            int si = (int)Math.Round(S);
            float q_diff = Math.Abs(qi - Q);
            float r_diff = Math.Abs(ri - R);
            float s_diff = Math.Abs(si - R);

            if (q_diff > r_diff && q_diff > s_diff)
            {
                qi = -ri - si;
            }
            else if (r_diff > s_diff)
            {
                ri = -qi - si;
            }
            else
            {
                si = -qi - ri;
            }
            return new Hex(qi, ri, si);
        }
    }
}

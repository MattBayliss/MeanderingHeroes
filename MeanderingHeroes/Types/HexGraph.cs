using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types
{
    public readonly record struct HexCoordinates(int X, int Y)
    {
        public static implicit operator (int, int)(HexCoordinates coords) => (coords.X, coords.Y);
        public static implicit operator Coordinate2D(HexCoordinates coords) => new Coordinate2D(coords.X, coords.Y, OffsetTypes.OddRowsRight);
        public static implicit operator Coordinate3D(HexCoordinates coords) => ((Coordinate2D)coords).To3D();
        public static implicit operator HexCoordinates(Coordinate2D d2) => new HexCoordinates(d2.X, d2.Y);
        public static implicit operator HexCoordinates(Coordinate3D d3) => (HexCoordinates)(d3.To2D(OffsetTypes.OddRowsRight));
    }
}

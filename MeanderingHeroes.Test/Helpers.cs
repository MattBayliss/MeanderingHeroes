using HexCore;
using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static LaYumba.Functional.F;

namespace MeanderingHeroes.Test
{
    internal static class Helpers
    {
        internal static Graph MakeHexMap(int hexColumns, int hexRows, Func<int, int, TerrainType> terrainForHex)
        {
            var cells = Range(1, hexRows)
                .SelectMany(r => Range(1, hexColumns).Select(c => (Row: r - 1, Col: c - 1)))
                .Select(
                    rc => new CellState(
                        false,
                        new Coordinate2D(rc.Row, rc.Col, OffsetTypes.OddRowsRight),
                        terrainForHex(rc.Row, rc.Col)
                    )
                ).ToList();

            return new Graph(cells, Constants.MovementAndTerrainTypes);
        }

        internal static T AssertIsSome<T>(Option<T> value)
        {
            if (value == None)
            {
                throw new ArgumentException($"Expected value to be Some, but was None");
            }
            return value.AsEnumerable().Single();
        }
    }
}

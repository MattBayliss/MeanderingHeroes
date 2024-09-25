using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Test
{
    internal static class Helpers
    {
        internal static Map MakeMap(int tilesX, int tilesY, Func<int, int, Terrain> terrainForCell)
        {
            Cell[,] cells = new Cell[tilesX, tilesY];
            Enumerable.Range(0, tilesX)
            .Select(x => Enumerable.Range(0, tilesY).Select(y => (x, y))).SelectMany(coords => coords)
            .ForEach(c => cells[c.x, c.y] = new Cell(terrainForCell(c.x, c.y)));

            return new Map(cells);
        }
    }
}

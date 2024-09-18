using LaYumba.Functional;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace MeanderingHeroes.Test
{
    public class MoveTests
    {
        private Map MakeMap(int tilesX, int tilesY, Func<int, int, Terrain> terrainForCell) {
            Cell[,] cells = new Cell[tilesX,tilesY];
            Enumerable.Range(0, tilesX)
            .Select(x => Enumerable.Range(0, tilesY).Select(y => (x, y))).SelectMany(coords => coords)
            .ForEach(c => cells[c.x, c.y] = new Cell(terrainForCell(c.x, c.y)));

            return new Map(cells);
        }

        [Fact]
        public void MoveEast()
        {
            // make a map 10 tiles wide, 3 tiles high
            var map = MakeMap(10, 3, (x,y) => Terrain.Grass);
            // make an intent to move to the east/right most tile, middle row
            var intent = new MoveIntent((9.0f, 1.0f));

            // make a hero that starts at the west/left-most tile, middle row
            // with the previously defined intent
            var hero = new Hero(1, "Testo", (0f, 1f), ImmutableList.Create(intent));

            var initialState = new GameState(map, ImmutableList.Create(hero));

            Enumerable.Range(0, 1000)
            .Aggregate(
                initialState,
                (state, _) => state.ActiveIntents()
                    .Aggregate(
                        state,
                        (s, herointent) => s.Run(herointent.Hero, herointent.Intent)
                    )
            );

        }
    }
}
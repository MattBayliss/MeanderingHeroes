using LaYumba.Functional;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

using static MeanderingHeroes.ModelLibrary;

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

            Location start = (0f, 1f);
            Location finish = (9f, 1f);

            // make a hero that starts at the west/left-most tile, middle row
            // with the previously defined intent
            var hero = new Hero(1, "Testo", start);

            // make an intent to move to the east/right most tile, middle row
            hero = hero.AddMoveIntent(finish);
            
            var initialState = new GameState(map, ImmutableList.Create(hero));

            // actual test, full of bad side-effects and whatnot
            int turnLimit = 100;
            int attempt = 0;

            Event? endEvent = null;

            var testFunc = (GameState state, ImmutableList<Event> events) =>
            {
                attempt++;
                if (attempt > turnLimit) return false;

                endEvent = events.FirstOrDefault(
                    ev => 
                        ev is EndEvent endev && endev.HeroId == hero.Id
                        && endev.Intent is MoveIntent mi && mi.Destination == finish
                );

                return endEvent != null;
            };

            var finalState = RunGame(initialState, testFunc);

            Assert.True(attempt < turnLimit);
            Assert.NotNull(endEvent);
        }
    }
}
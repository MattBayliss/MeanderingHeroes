using LaYumba.Functional;
using MeanderingHeroes.Functions;
using MeanderingHeroes.Types;
using System.Numerics;

namespace MeanderingHeroes.Test
{
    public class MoveTests
    {
        [Fact]
        public void OneTickTest()
        {
            var grid = new Grid(10, 10, (_, _) => new LandTerrain("grass", 1f));

            var hexStart = new Hex(1, 0);
            var hexDestination = new Hex(3, 0); // 2nd hex to the east

            // in cartesian
            var startAt = hexStart.Centre();
            var endAt = hexDestination.Centre();

            Assert.Equal(2f * Functions.Map.UnitsPerHex, endAt.X - startAt.X, 0.01f);
            Assert.Equal(0f, endAt.Y);
            Assert.Equal(0f, startAt.Y);

            // speed relative to hex centres
            var speed = 1f;

            var hero = new Hero(startAt, speed).AddGoal(new MoveGoal(endAt));
            Assert.Equal(startAt, hero.Location);

            hero = hero.Update();

            Assert.Equal(startAt with { X = startAt.X + speed }, hero.Location);
        }
        [Fact]
        public void PathToDestinationOnUniformTerrainTest()
        {
            var grid = new Grid(10, 10, (_, _) => new LandTerrain("grass", 1f));

            var hexStart = new Hex(0, 0); // top left of map
            var hexDestination = new Hex(3, 3); // 3 hexes to the SE

            // in cartesian
            var startAt = hexStart.Centre();
            var endAt = hexDestination.Centre();

            // speed relative to hex centres
            var speed = 1f;

            var hero = new Hero(startAt, speed).AddGoal(new MoveGoal(endAt));
            Assert.Equal(startAt, hero.Location);

            IEnumerable<Point> pathTaken = [];

            int quitAfterTick = 1000;
            int attempt = 1;

            while (hero.Location != endAt && attempt < quitAfterTick)
            {
                hero = hero.Update();
                pathTaken = pathTaken.Append(hero.Location);
                attempt++;
            }

            Assert.True(attempt < quitAfterTick);
            Assert.Equal(endAt, hero.Location);
            Assert.Equal(hexDestination, hero.Location.InHex());

            var distancesToEnd = pathTaken
                .Select(p => Vector2.Subtract(endAt, p).Length());

            // make a collection of tuples
            //  First: the distance to the destination for the current Point
            //  Second: the distance to the destination for the next Point
            // In this way we can test that we're moving toward the destination
            var distanceToEndForCurrentAndNext = distancesToEnd.Zip(distancesToEnd.Skip(1));

            // Assert there are no cases where the 2nd point is futher away than the first
            Assert.DoesNotContain(false, distanceToEndForCurrentAndNext.Select(tuple => tuple.First > tuple.Second));
        }

        [Fact]
        public void PathGoesAroundObstactle()
        {
            // make a 10x10 map where a mountain range blocks 0,4 to 6,4
            var terrainLayout = (int q, int r) =>
            {
                return (q, r) switch
                {
                    (var qq, var rr) when (rr == 4 && qq <= 6) => new LandTerrain("Mountain", 10f),
                    _ => new LandTerrain("grass", 1f)
                };
            };
            var grid = new Grid(10, 10, terrainLayout);

            // so if we start at 7,0, and try to end at 3,5, the hex route should be:
            // (7,0) => (7,1) => (7,2) => (7,3) => (7,4)  [just right of the mountain range]
            // => (7,5) => (6,5) => (5,5) => (4,5) => (3,5)

            var start = new Hex(7, 0);
            var end = new Hex(3, 5);

            Hex[] expectedRoute = [
                // heading down to 7,4, just east of the mountain range
                new Hex(7,1), new Hex(7,2), new Hex(7,3), new Hex(7,4), 
                // then diagonal / SW to 6,5
                new Hex(6,5),
                // then "west" to 3,5
                new Hex(5,5), new Hex(4,5), new Hex(3,5)
                ];

            var aStarRoute = grid.AStarPath(start, end).ToArray();

            Assert.Equal(expectedRoute, aStarRoute);
        }
    }
}
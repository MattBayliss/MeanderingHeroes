using LaYumba.Functional;
using MeanderingHeroes.Types;
using System.Numerics;

using static MeanderingHeroes.Functions;

namespace MeanderingHeroes.Test
{
    public class MoveTests
    {
        [Fact]
        public void OneTickTest()
        {
            var grid = Helpers.MakeGrass10x10MapGrid();

            var hexStart = Hex(1, 0);
            var hexDestination = Hex(3, 0); // 2nd hex to the east

            // in cartesian
            var startAt = hexStart.Centre();
            var endAt = hexDestination.Centre();

            Assert.Equal(2f * UnitsPerHex, endAt.X - startAt.X, 0.01f);
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
            var grid = Helpers.MakeGrass10x10MapGrid();

            var hexStart = Hex(0, 0); // top left of map
            var hexDestination = Hex(3, 3); // 3 hexes to the SE

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
            string smallMountainRangeAscii =
            //  0 1 2 3 4 5 6 7 8 9
                """
                _ _ _ _ _ _ _ _ _ _
                _ _ _ _ _ _ _ _ _ _
                _ _ _ _ _ _ _ _ _ _
                _ _ _ _ _ _ _ _ _ _
                M M M M M M M _ _ _
                _ _ _ _ _ _ _ _ _ _
                _ _ _ _ _ _ _ _ _ _
                """;
            var grid = Helpers.GenerateMapFromAsciiMess(smallMountainRangeAscii);

            var start = Hex(7, 0);
            var end = Hex(0, 5);

            Hex[] expectedRoute = [
                // heading down/south to 7,4, just east of the mountain range
                Hex(7,1), Hex(7,2), Hex(7,3), Hex(7,4), 
                // then diagonal / SW to 6,5
                Hex(6,5),
                // then "west" to 0,5
                Hex(5,5), Hex(4,5), Hex(3,5), Hex(2,5), Hex(1,5), Hex(0,5)
                ];

            var aStarRoute = grid.AStarPath(start, end).ToArray();

            Assert.Equal(expectedRoute, aStarRoute);
        }
    }
}
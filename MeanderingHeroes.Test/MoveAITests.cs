using LaYumba.Functional;
using MeanderingHeroes.Components;
using MeanderingHeroes.Types;
using System.Numerics;

using static MeanderingHeroes.Functions;

namespace MeanderingHeroes.Test
{
    /// <summary>
    /// Testing path-finding and Move related Considerations for the UtilityAI
    /// </summary>
    public class MoveAITests
    {
        [Fact]
        public void OneTickTest()
        {
            var grid = Helpers.MakeGrass10x10MapGrid();

            Hex hexStart = (1, 0);
            Hex hexDestination = (3, 0); // 2nd hex to the east

            // in cartesian
            var startAt = hexStart.Centre();
            var endAt = hexDestination.Centre();

            Assert.Equal(2f * UnitsPerHex, endAt.X - startAt.X, 0.01f);
            Assert.Equal(0f, endAt.Y);
            Assert.Equal(0f, startAt.Y);

            // speed relative to hex centres
            var speed = 1f;

            var moveConsideration = PathFinding.GeneratePathGoalConsideration(grid, hexStart, hexDestination);

            var hero = new Entity(startAt, speed).AddConsideration(moveConsideration);
            
            Assert.Equal(startAt, hero.Location);

            var utilityAI = new UtilityAIComponent(grid);
            hero = utilityAI.Update(new GameState([]), hero);

            Assert.Equal(startAt with { X = startAt.X + speed }, hero.Location);
        }
        [Fact]
        public void PathToDestinationOnUniformTerrainTest()
        {
            var grid = Helpers.MakeGrass10x10MapGrid();

            Hex hexStart = (0, 0); // top left of map
            Hex hexDestination = (3, 3); // 3 hexes to the SE

            // in cartesian
            var startAt = hexStart.Centre();
            var endAt = hexDestination.Centre();

            // distance relative to hex centres, per tick
            var speed = 0.3f;

            var moveConsideration = PathFinding.GeneratePathGoalConsideration(grid, hexStart, hexDestination);

            var hero = new Entity(startAt, speed).AddConsideration(moveConsideration);

            Assert.Equal(startAt, hero.Location);

            IEnumerable<Point> pathTaken = [];

            int quitAfterTick = 1000;
            int attempt = 1;

            var utilityAI = new UtilityAIComponent(grid);
            while (hero.Location != endAt && attempt < quitAfterTick)
            {
                hero = utilityAI.Update(new GameState([]), hero);
                pathTaken = pathTaken.Append(hero.Location);
                attempt++;
            }

            Assert.True(attempt < quitAfterTick);
            Assert.Equal(endAt, hero.Location);
            Assert.Equal(hexDestination, hero.Location.ToHex());

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

            Hex start = (7, 0);
            Hex end = (0, 5);

            Hex[] expectedRoute = [
                // heading down/south to 7,4, just east of the mountain range
                (7,1), (7,2), (7,3), (7,4), 
                // then diagonal / SW to 6,5
                (6,5),
                // then "west" to 0,5
                (5,5), (4,5), (3,5), (2,5), (1,5), (0,5)
                ];

            var aStarRoute = grid.AStarPath(start, end).ToArray();

            Assert.Equal(expectedRoute, aStarRoute);
        }
    }
}
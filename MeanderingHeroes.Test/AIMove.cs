using LaYumba.Functional;
using MeanderingHeroes.Engine;
using MeanderingHeroes.Engine.Types;
using System.Numerics;
using static LaYumba.Functional.F;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using MeanderingHeroes.Engine.Types.Behaviours;

namespace MeanderingHeroes.Test
{
    /// <summary>
    /// Testing path-finding and Move related Considerations for the UtilityAI
    /// </summary>
    public class AIMove(ITestOutputHelper output)
    {
        [Fact]
        public void OneTickTest()
        {
            var offsetZero = Vector2.Zero;

            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(offsetZero, 1f, 2f / MathF.Sqrt(3)),
                entities: []
            );

            Hex hexStart = (1, 0);
            Hex hexDestination = (3, 0); // 2nd hex to the east

            // in cartesian
            var startAt = game.HexCentreXY(hexStart);
            var endAt = game.HexCentreXY(hexDestination);

            Assert.Equal(2f, endAt.X - startAt.X, 0.001f);
            Assert.Equal(0f, endAt.Y);
            Assert.Equal(0f, startAt.Y);

            // speed relative to hex centres
            var speed = 1f;

            var moveBehaviour = BehavioursLibrary.PlayerSetDestination(hexDestination);

            var heroId = game.CreateEntity(hexStart, speed);
            game.AddBehaviour(heroId, moveBehaviour);

            var heroOpt = game[heroId];

            var hero = Helpers.AssertIsSome<Entity>(heroOpt);

            Assert.Equal(hexStart, hero.HexCoords);

            game.Update();

            hero = Helpers.AssertIsSome<Entity>(game.Entities.OfType<Entity>().Find(entity => entity.Id == hero.Id));

            var heroCartesian = game.HexCentreXY(hero.HexCoords);

            var expectedLocation = startAt with { X = startAt.X + speed };

            Assert.Equal(expectedLocation.Y, heroCartesian.Y, 0.001f);
            Assert.Equal(expectedLocation.X, heroCartesian.X, 0.001f);
        }
        [Fact]
        public void PathToDestinationOnUniformTerrainTest()
        {
            var offsetZero = Vector2.Zero;

            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(offsetZero, 1f, 2f / MathF.Sqrt(3)),
                entities: []
            );

            Hex hexStart = (1, 1); 
            Hex firstDest = (3, 3); // 2 hexes to the SE

            // in cartesian
            var startAt = game.HexCentreXY(hexStart);
            var endAt = game.HexCentreXY(firstDest);

            // distance relative to hex centres, per tick
            var speed = 0.3f;

            var moveBehaviour = BehavioursLibrary.PlayerSetDestination(firstDest);

            var heroId = game.CreateEntity(hexStart, speed);

            var baseDseIds = game.GetBehavioursForEntity(heroId).Select(dse => dse.Id).ToList();

            game.AddBehaviour(heroId, moveBehaviour);
            var hero = Helpers.AssertIsSome<Entity>(game[heroId]);
            Assert.Equal(hexStart, hero.HexCoords);

            IEnumerable<Vector2> pathTaken = [];

            int quitAfterTick = 1000;
            int attempt = 1;
            
            while (hero.HexCoords != firstDest && attempt < quitAfterTick)
            {
                game.Update();
                hero = Helpers.AssertIsSome<Entity>(game[heroId]);

                pathTaken = pathTaken.Append(game.HexCentreXY(hero.HexCoords));
                attempt++;
            }

            Assert.True(attempt < quitAfterTick);
            Assert.Equal(firstDest, hero.HexCoords);

            var distancesToEnd = pathTaken
                .Select(p => Vector2.Subtract(endAt, p).Length());

            // make a collection of tuples
            //  First: the distance to the destination for the current Point
            //  Second: the distance to the destination for the next Point
            // In this way we can test that we're moving toward the destination
            var distanceToEndForCurrentAndNext = distancesToEnd.Zip(distancesToEnd.Skip(1));

            // Assert there are no cases where the 2nd point is futher away than the first
            Assert.DoesNotContain(false, distanceToEndForCurrentAndNext.Select(tuple => tuple.First >= tuple.Second));

            var dseIdsAfter = game.GetBehavioursForEntity(heroId).Select(dse => dse.Id).ToList();

            Assert.Equal(baseDseIds, dseIdsAfter);

            Hex nextDest = (6, 6);
            endAt = game.HexCentreXY(nextDest);

            var nextDestinationBehaviour = BehavioursLibrary.PlayerSetDestination(nextDest);

            var nextDseId = game.AddBehaviour(heroId, nextDestinationBehaviour);

            hero = Helpers.AssertIsSome<Entity>(game[heroId]);

            // make sure position hasn't changed
            Assert.Equal(firstDest, hero.HexCoords);

            attempt = 1;

           while (hero.HexCoords != nextDest && attempt < quitAfterTick)
            {
                game.Update();
                hero = Helpers.AssertIsSome<Entity>(game[heroId]);

                pathTaken = pathTaken.Append(game.HexCentreXY(hero.HexCoords));
                attempt++;
            }

            output.WriteLine($"pathTaken: [{string.Join(",", pathTaken)}]");

            distancesToEnd = pathTaken
                .Select(p => Vector2.Subtract(endAt, p).Length());

            // make a collection of tuples
            //  First: the distance to the destination for the current Point
            //  Second: the distance to the destination for the next Point
            // In this way we can test that we're moving toward the destination
            distanceToEndForCurrentAndNext = distancesToEnd.Zip(distancesToEnd.Skip(1));

            // Assert there are no cases where the 2nd point is futher away than the first
            Assert.DoesNotContain(false, distanceToEndForCurrentAndNext.Select(tuple => tuple.First >= tuple.Second));
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

        [Fact]
        public void SpeedIsFractionsOfHexWidth()
        {
            var offsetZero = Vector2.Zero;

            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(offsetZero, 1f, 1f),
                entities: []
            );

            // ensure assumption that all terrain costs are 1.0
            Assert.DoesNotContain(false, game.HexMap.Terrain.Values.Select(t => t.MovementCost == 1.0));

            // distance relative to hex centres, per tick
            var speed = 1f;

            Hex hexStart = (0, 0);
            Hex hexDestination = (2, 0); // 2 "speeds", just so we can see it stop at (1,0)

            // in cartesian
            var startAt = game.HexCentreXY(hexStart);
            var endAt = game.HexCentreXY(hexDestination);

            var moveBehaviour = BehavioursLibrary.PlayerSetDestination(hexDestination);

            var heroId = game.CreateEntity(hexStart, speed);
            game.AddBehaviour(heroId, moveBehaviour);

            var heroOpt = game[heroId];

            var hero = Helpers.AssertIsSome<Entity>(heroOpt);
            Assert.Equal(hexStart, hero.HexCoords.Round());

            game.Update();
            var heroAfter1Tick = Helpers.AssertIsSome<Entity>(game.Entities.OfType<Entity>().Find(entity => entity.Id == hero.Id));

            var atHex = heroAfter1Tick.HexCoords;

            Assert.Equal(new Hex(1, 0), atHex.Round());
            Assert.Equal(new FractionalHex(1f, 0f), atHex);
        }

        [Fact]
        public void TerrainCostsAffectDistanceTravelled()
        {
            var grid = new Grid(
                Range(0, 9)
                    .Select(r => (
                        Hex: new Hex(r, 0),
                        Terrain: (Terrain)(r < 5 ? new LandTerrain("grass", 1f) : new LandTerrain("forest", 3f))))
                );

            Hex hexStart = (0, 0); // left-most
            Hex hexDestination = (9, 0); // right-most

            var offsetZero = Vector2.Zero;

            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: grid,
                transforms: new Transforms(offsetZero, 1f, 2f / MathF.Sqrt(3)),
                entities: []
            );

            // in cartesian
            var startAt = game.HexCentreXY(hexStart);
            var endAt = game.HexCentreXY(hexDestination);

            // distance relative to hex centres, per tick (easily divisible by 3 for occular pat down)
            var speed = 0.1f;

            var moveBehaviour = BehavioursLibrary.PlayerSetDestination(hexDestination);

            var heroId = game.CreateEntity(hexStart, speed);
            game.AddBehaviour(heroId, moveBehaviour);

            int attemptLimit = 1000;
            int attempt = 1;

            IEnumerable<Vector2> pointsAlongPath = [];

            var heroOpt = game[heroId];
            var hero = Helpers.AssertIsSome<Entity>(heroOpt);

            while (hero.HexCoords != hexDestination && attempt < attemptLimit)
            {
                game.Update();
                hero = Helpers.AssertIsSome<Entity>(game[heroId]);

                pointsAlongPath = pointsAlongPath.Append(game.ToGameXY(hero.HexCoords));
                attempt++;
            }

            Assert.True(attempt < attemptLimit);
            Assert.True(pointsAlongPath.Count() > 1);
            Assert.Equal(endAt, pointsAlongPath.Last());

            var hexesAndDistanceTravelled = pointsAlongPath
                .Zip(pointsAlongPath.Skip(1))
                .Select(points => (
                    FromHex: game.HexQR(points.First).Round(),
                    ToHex: game.HexQR(points.Second).Round(),
                    DistanceTravelled: Vector2.Distance(points.First, points.Second)))
                .Select(ftd =>
                (
                    FromHex: ftd.FromHex,
                    FromTerrain: game.HexMap.TerrainForHex(ftd.FromHex),
                    ToHex: ftd.ToHex,
                    ToTerrain: game.HexMap.TerrainForHex(ftd.ToHex),
                    DistanceTravelled: ftd.DistanceTravelled
                ))
                .SkipLast(1); // the last hop will be to the destination - a remainder value we can ignore for this test

            hexesAndDistanceTravelled.ForEach(had =>
            {
                var fromTerrain = Helpers.AssertIsSome(had.FromTerrain);
                var toTerrain = Helpers.AssertIsSome(had.ToTerrain);

                // if travelling through the same terrain type, the distance travelled is a simple calculation
                // of speed / movement cost
                if (fromTerrain == toTerrain)
                {
                    if (had.ToHex.Q < 5)
                    {
                        Assert.Equal(1f, toTerrain.MovementCost);
                    }
                    else if (had.FromHex.Q >= 5)
                    {
                        Assert.Equal(3f, fromTerrain.MovementCost);
                    }
                    Assert.Equal(speed / fromTerrain.MovementCost, had.DistanceTravelled, 0.001);
                }
                else
                {
                    // because we made a map where movement cost increases, we don't need to sort from/to movement costs
                    Assert.InRange(had.DistanceTravelled, speed / toTerrain.MovementCost, speed / fromTerrain.MovementCost);
                }
            });
        }
    }
}
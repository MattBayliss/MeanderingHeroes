using System.Numerics;
using MeanderingHeroes.Engine.Types;
using MeanderingHeroes.Engine.Types.Behaviours;
using MeanderingHeroes.Test;
using Xunit.Abstractions;

namespace MeanderingHeroes.Test
{
    public class DecisionTests(ITestOutputHelper output)
    {
        [Fact]
        public void PathFindingTests()
        {
            var offsetZero = Vector2.Zero;

            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(offsetZero, 1f, 2f / MathF.Sqrt(3)),
                entities: []
            );

            Hex hexStart = (1, 0);
            Hex hexDestination1 = (3, 0); // 2nd hex to the east

            var heroId = game.CreateEntity(hexStart, 0.1f);

            var hero = Helpers.AssertIsSome<Entity>(game[heroId]);

            Assert.Equal(hexStart, hero.HexCoords);

            var considerationContext = new ConsiderationContext(game);
            considerationContext.SetStateSnapshot();
            var dest1Behaviour = BehavioursLibrary.PlayerSetDestination(hexDestination1)(game)(hero);
            var decision = Assert.Single(dest1Behaviour.Dse.Decisions);
            var distanceConsideration = considerationContext.GetConsideration(decision)(hero);

            Assert.True(distanceConsideration > 0);
        }
    }
}
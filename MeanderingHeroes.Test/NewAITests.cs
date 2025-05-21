using MeanderingHeroes.Engine.Types;
using System.Numerics;
using Xunit.Abstractions;
using static MeanderingHeroes.Test.Helpers;

namespace MeanderingHeroes.Test
{
    public class NewAITests(ITestOutputHelper output)
    {
        [Fact]
        public void PlayerSetDestinationTest()
        {
            var offsetZero = Vector2.Zero;

            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(offsetZero, 1f, 2f / MathF.Sqrt(3)),
                entities: []
            );

            var start = new Hex(1,1);
            var destination = new Hex(9, 9);
            var player = game.CreateEntity(start, 0.3f);
            
            game.AddBehaviour(player, BehavioursLibrary.PlayerSetDestination(destination));

            game.Update();

            var updatedPlayer = AssertIsSome<Entity>(game[player.Id]);
            var newCoords = updatedPlayer.HexCoords;

            FractionalHex destCoords = destination;

            Assert.True(destCoords.Distance(start) > destCoords.Distance(newCoords));
        }
    }
}

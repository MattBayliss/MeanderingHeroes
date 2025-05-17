using MeanderingHeroes.Engine.Types;
using System.Numerics;

namespace MeanderingHeroes.Test
{
    public class AIAdvertise
    {
        [Fact]
        public void TreasureAdvertises()
        {
            var game = new Game(
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(Vector2.Zero, 1f, 2f / MathF.Sqrt(3)),
                entities: []
            );

            // very slow hero, in the same hex as a "treasure"
            var hero = game.CreateEntity((6.0f, 6.0f), 0.01f);

        }
    }
}

using MeanderingHeroes.Engine.Components;
using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
            var hero = game.CreateSmartEntity((6.0f, 6.0f), 0.01f);

            var treasure = game.CreateAdvertiser(
                hexCoords: (6.1f, 6.4f),
                offers:
                    [
                        new Interaction(ConsideratonsLibrary.AcquireWealth, CurvesLibrary.Linear(0f,1f,1f))
                    ]
            );


        }
    }
}

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

            var hero = game.CreateSmartEntity((0f, 0f), 1f);

            var treasure = game.CreateAdvertiser(
                hexCoords: (6f, 6f),
                offers:
                    [
                        //new Offer(
                        //    Types: AdvertFlag.Greed | AdvertFlag.Wealth,
                        //    Consideration:
                        //    )
                    ]
            );


        }
    }
}

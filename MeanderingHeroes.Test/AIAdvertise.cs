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
                HexMap: Helpers.MakeGrass10x10MapGrid(),
                Transforms: new Transforms(Vector2.Zero, 1f, 2f / MathF.Sqrt(3)),
                Entities: []
            );

            var treasure = new Advertiser(
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

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
    public class AIUtilityLibrary
    {
        [Fact]
        public void PlayerUtilityThatDecaysPerTick()
        {
            
            // make a simple linear decay function:
            Func<int, Utility> decayFunc = tick => tick > 100 ? 0f : (100f - tick) / 100f;

            var decayUtility = UtilityLibrary.PlayerUtility(decayFunc);

            var game = new Game(new Grid([]), new Transforms(Vector2.Zero, 1, 1), []);
            var noEntity = new SmartEntity(1, (0, 0), 0);

            for(int t = 0; t < 110; t++)
            {
                Assert.Equal(decayFunc(t), decayUtility(game, noEntity));
            }
        }
    }
}

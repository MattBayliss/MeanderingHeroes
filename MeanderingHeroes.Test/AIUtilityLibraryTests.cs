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
    public class AIUtilityLibraryTests
    {
        [Fact]
        public void PlayerUtilityThatDecaysPerTick()
        {
            
            // make a simple linear decay function:
            Func<int, Utility> decayFunc = tick => tick > 100 ? 0f : (100f - tick) / 100f;

            var decayUtility = UtilityLibrary.PlayerUtility(decayFunc);

            var game = new Game(new Grid([]), new Transforms(Vector2.Zero, 1, 1), []);
            var noEntity = game.CreateSmartEntity((0, 0), 0);

            var decayedPerTick = LaYumba.Functional.F.Range(0, 109).Select(_ => decayUtility(game, noEntity)).ToArray();

            for(int t = 0; t < 110; t++)
            {
                Assert.Equal(decayFunc(t), decayedPerTick[t]);
            }
            for (int t = 1; t < 100; t++)
            {
                Assert.True(decayedPerTick[t] < decayedPerTick[t-1]);
            }
        }
        [Fact]
        public void DistanceUtilityTest()
        {
            FractionalHex firstHex = (0, 0);
            FractionalHex targetHex = (3, 0);
            var game = new Game(new Grid([]), new Transforms(Vector2.Zero, 1, 1), []);
            var movingEntity = game.CreateSmartEntity(firstHex, 0);
            var targetEntity = game.CreateAdvertiser(targetHex, []);

            Func<float, Utility> distanceCurve = distance => distance / 10f;

            var distanceToTargetUtitily = UtilityLibrary.DistanceUtility(targetEntity, distanceCurve);

            Assert.Equal<Utility>(firstHex.Distance(targetHex) / 10f, distanceToTargetUtitily(game, movingEntity));

            FractionalHex closerHex = (2, 0);

            var closerEntity = game.CreateSmartEntity(closerHex, 0);
            var firstDistance = movingEntity.HexCoords.Distance(targetEntity.HexCoords) / 10f;

            Assert.Equal<Utility>(closerHex.Distance(targetHex) / 10f, distanceToTargetUtitily(game, closerEntity));

        }
    }
}

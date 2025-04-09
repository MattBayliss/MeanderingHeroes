using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Test
{
    public class Types
    {
        [Fact]
        public void UtilityMustBeBetween0and1()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { Utility utility = -0.1f; });
            Assert.Throws<ArgumentOutOfRangeException>(() => { Utility utility = 1.1f; });

            var randomFloatBetween0And1 = Random.Shared.NextSingle();

            Utility validUtility = randomFloatBetween0And1;
            Assert.Equal(randomFloatBetween0And1, (float)validUtility);
        }
    }
}

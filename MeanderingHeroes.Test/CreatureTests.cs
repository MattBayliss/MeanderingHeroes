using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Location = System.Numerics.Vector2;

using static MeanderingHeroes.ModelLibrary;
using static MeanderingHeroes.Test.Helpers;
using static LaYumba.Functional.F;


namespace MeanderingHeroes.Test
{
    public class CreatureTests
    {
        [Fact]
        public void HeroHuntDeerTest()
        {
            Assert.Fail();
            var map = MakeMap(20, 20, (x, y) => Terrain.Grass);

            var deer = new Beast(1, "deer", new Location(4.0f, 5.0f))
                .AddFleeReaction(0.3f);

            var hunter = new Hero(2, "Pete", new Location(0.0f, 5.0f));
        }
    }
}

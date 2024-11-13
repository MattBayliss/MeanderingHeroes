using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Location = System.Numerics.Vector2;

using static MeanderingHeroes.ModelLibrary;
using static MeanderingHeroes.Test.Helpers;
using static LaYumba.Functional.F;
using MeanderingHeroes.Types.Doers;
using MeanderingHeroes.Types.Commands;


namespace MeanderingHeroes.Test
{
    public class CreatureTests
    {
        [Fact]
        public void HeroHuntDeerTest()
        {
            var map = MakeMap(20, 20, (x, y) => Terrain.Grass);

            var deer = new Beast("deer", new Location(4.0f, 5.0f))
                .AddFleeReaction(0.3f);

            var hunter = new Hero("Pete", new Location(0.0f, 5.0f));

            var initialState = new GameState(map)
                .Add(hunter)
                .Add(deer)
                .AddHuntIntent(hunter);

            Assert.Fail();
        }
    }
}

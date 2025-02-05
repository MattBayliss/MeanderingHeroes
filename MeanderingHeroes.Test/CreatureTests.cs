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
using MeanderingHeroes.Types;


namespace MeanderingHeroes.Test
{
    public class CreatureTests
    {
        public void HeroHuntDeerTest()
        {
            var map = MakeHexMap(20, 20, (x, y) => Constants.GrassTerrain);

            var deer = new Beast("deer", new HexCoordinates(4,5))
                .AddFleeReaction(0.3f);

            var hunter = new Hero("Pete", new HexCoordinates(0, 5));

            var initialState = new GameState(map)
                .Add(hunter)
                .Add(deer)
                .AddHuntIntent(hunter);

            // TODO: everything
        }
    }
}

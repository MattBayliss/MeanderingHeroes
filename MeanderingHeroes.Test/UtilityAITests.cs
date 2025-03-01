using MeanderingHeroes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MeanderingHeroes.Functions;

namespace MeanderingHeroes.Test
{
    public class UtilityAITests
    {
        [Fact]
        public void SimpleDestinationTest()
        {
            var grid = Helpers.MakeTestMap2Grid();

            // start at the bottom left of the map,
            Hex start = (4, 16);
            // and head to the top right
            Hex end = (28, 4);
            var speed = UnitsPerHex;

            var gamestate = new GameState([
                new Hero(start.Centre(), speed).AddGoal(new MoveGoal(end.Centre()))
            ]);
        }
    }
}

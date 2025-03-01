using MeanderingHeroes.Functions;
using MeanderingHeroes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Test
{
    public class UtilityAITests
    {
        [Fact]
        public void SimpleDestinationTest()
        {
            var grid = Helpers.MakeTestMap2Grid();

            // start at the bottom left of the map,
            var start = new Hex(4, 16);
            // and head to the top right
            var end = new Hex(28, 4);
            var speed = 1f;

            var gamestate = new GameState([
                new Hero(start.Centre(), speed).AddGoal(new MoveGoal(end.Centre()))
            ]);
        }
    }
}

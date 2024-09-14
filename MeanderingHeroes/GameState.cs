using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes
{
    public record GameState(Map map, IEnumerable<Hero> Heroes)
    {

        // need to track:
        // map -> grid -> cell (weather, terrain, travel costs (n,s,e,w))
        // monsters -> monster (location, flags (hungry, scared, wounded, aggressive))
        // heroes -> hero (location, instructions, 
    }

    public static class GameStateLibrary
    {
        public static GameState DoTurn(this GameState current, IEnumerable<Func<GameState, GameState>> instructions)
        {
            return instructions.Aggregate(
                current,
                (last, update) => update(last)
            );
        }
    }
}

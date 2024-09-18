using LaYumba.Functional;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using static MeanderingHeroes.ModelLibrary;

namespace MeanderingHeroes
{
    public class Game
    {
        public Game(
            GameState initialState,
            Func<Hero, GameState, double> GetHeroSpeed
            )
        { }
    }

    public class GameRunner
    {
        public IEnumerable<int> Turns(Func<bool> endCondition) { 
            while(!endCondition())
            {
                yield return 1;
            }
        }
    }


    // need to track:
    // map -> grid -> cell (weather, terrain, travel costs (n,s,e,w))
    // monsters -> monster (location, flags (hungry, scared, wounded, aggressive))
    // heroes -> hero (location, instructions, 
    public readonly record struct GameState(Map map, ImmutableList<Hero> Heroes);

    public static class GameStateLibrary
    {
        public static GameState DoTurn(this GameState current, IEnumerable<Func<GameState, GameState>> instructions)
        {
            return instructions.Aggregate(
                current,
                (last, update) => update(last)
            );
        }

        // later, add events:
        // public static (GameState, ImmutableList<Event>) Run
        public static GameState Run(this GameState current, Hero hero, HeroIntent intent) =>
            current with { Heroes = current.Heroes.Replace(hero, intent.HeroComputation(current, hero)) };
    }
}

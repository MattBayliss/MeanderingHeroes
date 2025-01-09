using LaYumba.Functional;
using MeanderingHeroes.Types.Commands;
using MeanderingHeroes.Types.Doers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types
{
    // need to track:
    // map -> grid -> cell (weather, terrain, travel costs (n,s,e,w))
    // monsters -> monster (location, flags (hungry, scared, wounded, aggressive))
    // heroes -> hero (location, instructions, 
    public readonly record struct GameState
    {
        public Map Map { get; init; }
        public ImmutableList<Doer> Doers { get; init; }
        public ImmutableList<Command> Commands { get; init; }
        public GameState(Map map)
        {
            Map = map;
            Doers = [];
            Commands = [];
        }
    }

    // GameState Functions
    public static partial class ModelLibrary
    {
        public static GameState Add(this GameState state, Doer doer)
        {
            return state with { Doers = state.Doers.Add(doer) };
        }
        public static GameState Add(this GameState state, Command command)
        {
            return state with { Commands = state.Commands.Add(command) };
        }
        public static Option<T> GetDoer<T>(this GameState state, int id) where T : Doer
        {
            return state.Doers.OfType<T>().Find(d => d.Id == id);
        }
    }
}

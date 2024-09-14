using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes
{
    public record Hero(int Id, Name Name, Location Location, Condition Condition, IEnumerable<Intent> Intents, IEnumerable<Event> Events);

    // placeholders while I figure stuff out
    public record Name;
    public record Location;
    public record Condition(Health Health, Hunger Hunger, Energy Energy);
    public record Health;
    public record Hunger;
    public record Energy;

    // instructions affect state on many items... Hunt affects Hunter and Prey...
    // thought: Intents translated to -> Activity, which cause -> Events (seen/heard/chased/fled) -> Listeners in the form of other actors (heroes, monsters, groups)
    public record Intent;
    public record Activity;
    public record Event;

    public enum InstructionType
    {
        Rest,
        Forage,
        Hunt,
        Goto,
        Fight,
        Flight,
        Recuperate,
        Hide
    }

    public static class HeroLibrary
    {
        public static Hero Move(this Hero hero, Map map)
        {
            // move logic goes here
            return hero;
        }
    }
}

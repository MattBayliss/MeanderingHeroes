using System;
using System.Collections;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes
{
    public record Hero(int Id, Name Name, Location Location, ImmutableList<HeroIntent> Intents); //, Condition Condition);

    // placeholders while I figure stuff out
    public record Name(string Value)
    {
        public static implicit operator Name(string name) => new Name(name);
        public static implicit operator string(Name name) => name.Value;
    }
    public record Location(float X, float Y)
    {
        public static implicit operator Location((float x, float y) tuple) => new Location(tuple.x, tuple.y);
        public static implicit operator Vector2(Location location) => new Vector2(location.X, location.Y);
        public static implicit operator Location(Vector2 vector) => new Location(vector.X, vector.Y);
    }

    public record Condition(Health Health, Hunger Hunger, Energy Energy);
    public record Health;
    public record Hunger;
    public record Energy;

    // instructions affect state on many items... Hunt affects Hunter and Prey...
    // thought: Intents translated to -> Activity, which cause -> Events (seen/heard/chased/fled) -> Listeners in the form of other actors (heroes, monsters, groups)
    // intent > Activity > Event[]
    public record Activity;
    public record Event;

    public enum IntentType
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

}

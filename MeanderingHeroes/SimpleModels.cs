using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes
{
    public record Map(Cell[,] Cells);

    public record Cell(Terrain Terrain); //, Weather CurrentWeather, ReadOnlyDictionary<Direction, TravelCost> TravelCosts);
    public record Weather;
    public enum Terrain
    {
        Grass,
        Forest,
        Swamp
    };
    public record TravelCost;
    public enum Direction { North, South, East, West }

    public abstract class Intent : IEqualityComparer<Intent>
    {
        public virtual bool Equals(Intent? x, Intent? y) => x?.Id == y?.Id;
        public virtual int GetHashCode([DisallowNull] Intent obj) => obj.Id.GetHashCode();

        public long Id { get; init; }
        public Intent() => Id = DateTime.UtcNow.Ticks;
    }

    public abstract class HeroIntent : Intent
    {
        public int HeroId { get; init; }
        public abstract Func<GameState, Hero, (Hero, Events)> HeroComputation { get; init; }
        public HeroIntent(int heroId) : base()
        {
            HeroId = heroId;
        }
        public override bool Equals(Intent? x, Intent? y) =>
            (x, y) switch
            {
                (null, null) => true, (null, _) => false, (_, null) => false,
                (HeroIntent hx, HeroIntent hy) => (hx.HeroId, hx.Id) == (hy.HeroId, hy.Id),
                _ => false
            };

        public override int GetHashCode() => (HeroId, Id).GetHashCode();
    }

    public record Hero
    {
        public int Id { get; init; }
        public Name Name { get; init; }
        public Location Location { get; init; }
        public HeroIntents Intents { get; init; }

        public Hero(int id, Name name, Location location)
        {
            Id = id;
            Name = name;
            Location = location;
            Intents = ImmutableList<HeroIntent>.Empty;
        }
    }

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

    public record Activity;

    public abstract record Event
    {
        public DateTime DateTimeStamp { get; init; }
        public Event() => DateTimeStamp = DateTime.UtcNow;
    }
    public record ArrivedEvent(int HeroId, Location location) : Event();
    public record EndEvent(int HeroId, HeroIntent Intent) : Event();

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

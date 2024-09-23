﻿using System;
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

    public record Turn<T>
    {
        public T Value { get; init; }
        public Turn(T value) => Value = value;
        public static implicit operator T (Turn<T> turn) => turn.Value;
        public static implicit operator Turn<T>(T value) => new Turn<T>(value);
    }
    public record Done<T> : Turn<T>
    {
        public Done(T value) : base(value) { }
    }

    public static partial class ModelLibrary
    {
        public static Done<T> Done<T>(T value) => new Done<T>(value);
        public static Turn<T> Turn<T>(T value) => new Turn<T>(value);
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

    public record Creature
    {
        public int Id { get; init; }
        public Location Location { get; init; }

        public Creature(int id, Location location)
        {
            Id = id;
            Location = location;
        }
    }
    public record Hero : Creature
    {
        public Name Name { get; init; }
        public HeroIntents Intents { get; init; }

        public Hero(int id, Name name, Location location) : base(id, location)
        {
            Id = id;
            Name = name;
            Location = location;
            Intents = [];
        }
    }

    // placeholders while I figure stuff out
    public record Name(string Value)
    {
        public static implicit operator Name(string name) => new Name(name);
        public static implicit operator string(Name name) => name.Value;
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
    public abstract record LocationEvent:Event
    {
        public Location Location { get; init; }
        public LocationEvent(Location location) : base() => Location = location;
    }
    public record ArrivedEvent(int HeroId, Location location) : LocationEvent(location);
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

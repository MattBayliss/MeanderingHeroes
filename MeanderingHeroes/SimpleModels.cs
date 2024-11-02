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
    public delegate (Doers, Events) EventAction(Event sourceEvent);
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

    public class Intent : IEqualityComparer<Intent>
    {
        public virtual bool Equals(Intent? x, Intent? y) => x?.Id == y?.Id;
        public virtual int GetHashCode([DisallowNull] Intent obj) => obj.Id.GetHashCode();
        public virtual Func<GameState, Doer, (Doer Doer, Events Events)> ProcessIntent { get; init; }

        public long Id { get; init; }
        public Intent()
        {
            Id = DateTime.UtcNow.Ticks;
            ProcessIntent = (_, d) => (d, []);
        }
    }

    // (GameState, Event) => (GameState, Event)
    // 
    public abstract record Trigger
    {
        public required virtual float Threshold { get; init; }
    }
    public record ProximityTrigger : Trigger;
    public record ThreatTrigger : Trigger;
    public record Reaction
    {
        public required virtual Triggers Triggers { get; init; }
        public required virtual EventAction Action { get; init; }
    }

    public record Flee : Reaction;
    



    /// <summary>
    /// Simple discriminated union Turn <typeparamref name="T"/> | Done <typeparamref name="T"/>
    /// Turn : Updated State of the Intent.
    /// Done : Final State of the Intent, signaling that the Intent is over.
    /// </summary>
    /// <typeparam name="T">The type of the state of the Intent i.e. `Location` for a MoveIntent</typeparam>
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

    public abstract class DoerIntent : Intent
    {
        public int DoerId { get; init; }
        public DoerIntent(int doerId) : base()
        {
            DoerId = doerId;
        }
        public override bool Equals(Intent? x, Intent? y) =>
            (x, y) switch
            {
                (null, null) => true, (null, _) => false, (_, null) => false,
                (DoerIntent hx, DoerIntent hy) => (hx.DoerId, hx.DoerId) == (hy.DoerId, hy.DoerId),
                _ => false
            };

        public override int GetHashCode() => (DoerId, DoerId).GetHashCode();
    }
    /// <summary>
    /// Base class for all heroes, monsters, animals, etc
    /// Hard to think of a name that captures all that.
    /// Settling on "Doer" for now - something that does things
    /// </summary>
    public record Doer
    {
        public int Id { get; init; }
        public Location Location { get; init; }
        public ImmutableList<Reaction> Reactions { get; init; }
        public IntentList Intents { get; init; }

        public Doer(int id, Location location)
        {
            Id = id;
            Location = location;
            Intents = [];
            Reactions = [];
        }
    }
    public record Beast : Doer
    {
        public string Species { get; init; }
        public Beast(int id, string species, Location location) : base(id, location)
        {
            Species = species;
        }
    }
    public record Hero : Doer
    {
        public Name Name { get; init; }

        public Hero(int id, Name name, Location location) : base(id, location)
        {
            Name = name;
        }
    }

    // placeholders while I figure stuff out
    
    /// <summary>
    /// Pretty much a descriptive alias for string, but will
    /// have validation rules later (because player's input will be weird)
    /// </summary>
    /// <param name="Value"></param>
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
    public record DoerEvent : Event
    {
        public required int DoerId { get; init; }
    }
    public abstract record LocationEvent : DoerEvent
    {
        public required Location Location { get; init; }
    }
    public record ArrivedEvent : LocationEvent
    {
        public static ArrivedEvent Create(int doerId, Location location)
            => new()
            {
                DoerId = doerId,
                Location = location
            };
    }
    public record EndEvent : DoerEvent
    {
        public required Intent Intent { get; init; }
        public static EndEvent Create(int doerId, Intent intent)
            => new()
            {
                DoerId = doerId,
                Intent = intent
            };
    }
}

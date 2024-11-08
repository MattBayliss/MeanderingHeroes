using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Models.Doers
{
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

}

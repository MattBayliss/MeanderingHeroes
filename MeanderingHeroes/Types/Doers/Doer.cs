using LaYumba.Functional;
using MeanderingHeroes.Functions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types.Doers
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

        public Doer(Location location)
        {
            Id = UniqueIds.NextDoerId;
            Location = location;
            Reactions = [];
        }
    }
    public record Beast : Doer
    {
        public string Species { get; init; }
        public Beast(string species, Location location) : base(location)
        {
            Species = species;
        }
    }
    public record Hero : Doer
    {
        public Name Name { get; init; }

        public Hero(Name name, Location location) : base(location)
        {
            Name = name;
        }
    }

}

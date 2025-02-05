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
        public HexCoordinates Location { get; init; }
        public ImmutableList<Reaction> Reactions { get; init; }

        public Doer(HexCoordinates location)
        {
            Id = UniqueIds.NextDoerId;
            Location = location;
            Reactions = [];
        }
    }
    public record Beast : Doer
    {
        public string Species { get; init; }
        public Beast(string species, HexCoordinates location) : base(location)
        {
            Species = species;
        }
    }
    public record Hero : Doer
    {
        public Name Name { get; init; }
        public Strength Strength { get; init; }
        public Dexterity Dexterity { get; init; }
        public Willpower Willpower { get; init; }

        public Hero(Name name, HexCoordinates location) : base(location)
        {
            Name = name;
        }
    }

}

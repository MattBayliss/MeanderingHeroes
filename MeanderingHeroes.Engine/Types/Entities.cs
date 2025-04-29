using MeanderingHeroes.Engine.Components;
using Xunit;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record Entity
    {
        public Hex Hex { get; init; }
        public FractionalHex HexCoords { get; init; }
        public int Id { get; private init; }
        public ImmutableList<Behaviour> Behaviours { get; init; }
        internal Entity(int id, FractionalHex hexCoords) : this(id, hexCoords, []) { }
        internal Entity(int id, FractionalHex hexCoords, IEnumerable<Behaviour> behaviours)
        {
            Id = id;
            HexCoords = hexCoords;
            Hex = hexCoords.Round();
            Behaviours = behaviours.ToImmutableList();
        }
        public override int GetHashCode() => Id;
    }
    public record Advertiser : Entity
    {
        internal Advertiser(int id, FractionalHex hexCoords, IEnumerable<Behaviour> behaviours) : base(id, hexCoords, behaviours) { }
    }
    public record SmartEntity : Advertiser
    {
        public float Speed { get; init; } = 0F;
        //TODO: Add some default Offers to a SmartEntity
        internal SmartEntity(int id, FractionalHex hexCoords, float speed, IEnumerable<Behaviour> behaviours) : base(id, hexCoords, behaviours)
        {
            Assert.True(speed >= 0);

            HexCoords = hexCoords;
            Speed = speed;
        }
    }
}

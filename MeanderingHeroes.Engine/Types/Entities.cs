using MeanderingHeroes.Engine.Components;
using Xunit;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record Entity
    {
        public FractionalHex HexCoords { get; init; }
        public int Id { get; private init; }
        internal Entity(int id, FractionalHex hexCoords)
        {
            Id = id;
            HexCoords = hexCoords;
        }
        public override int GetHashCode() => Id;
    }
    public record Advertiser : Entity
    {
        public ImmutableList<InteractionBase> Offers { get; init; }
        internal Advertiser(int id, FractionalHex hexCoords, IEnumerable<InteractionBase> offers) : base(id, hexCoords)
        {
            Offers = offers.ToImmutableList();
        }
    }
    public record SmartEntity : Advertiser
    {
        public float Speed { get; init; } = 0F;
        public ImmutableList<Behaviour> Behaviours { get; init; } = [];
        //TODO: Add some default Offers to a SmartEntity
        internal SmartEntity(int id, FractionalHex hexCoords, float speed) : base(id, hexCoords, [])
        {
            Assert.True(speed >= 0);

            HexCoords = hexCoords;
            Speed = speed;
        }
    }
}

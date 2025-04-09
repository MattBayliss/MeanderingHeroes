using MeanderingHeroes.Engine.Components;
using Xunit;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record Entity
    {
        public FractionalHex HexCoords { get; init; }
        public int Id { get; private init; }
        protected Entity(int id, FractionalHex hexCoords)
        {
            Id = id;
            HexCoords = hexCoords;
        }
        public override int GetHashCode() => Id;
    }
    public record Advertiser : Entity
    {
        public ImmutableList<Offer> Offers { get; init; }
        public Advertiser(int id, FractionalHex hexCoords, IEnumerable<Offer> offers) : base(id, hexCoords)
        {
            Offers = offers.ToImmutableList();
        }
    }
    public record SmartEntity : Advertiser
    {
        public float Speed { get; init; } = 0F;
        public IImmutableList<IConsideration> Considerations { get; init; } = [];
        //TODO: Add some default Offers to a SmartEntity
        public SmartEntity(int id, FractionalHex hexCoords, float speed) : base(id, hexCoords, [])
        {
            Assert.True(speed >= 0);

            HexCoords = hexCoords;
            Speed = speed;
        }
    }
}

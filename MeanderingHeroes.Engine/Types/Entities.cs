using MeanderingHeroes.Engine.Components;
using Xunit;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record Entity(FractionalHex HexCoords);
    public record Advertiser : Entity
    {
        public ImmutableList<Offer> Offers { get; init; }
        public Advertiser(FractionalHex hexCoords, IEnumerable<Offer> offers) : base(hexCoords)
        {
            Offers = offers.ToImmutableList();
        }
    }
    public record SmartEntity : Advertiser
    {
        public float Speed { get; init; } = 0F;
        public IImmutableList<IConsideration> Considerations { get; init; } = [];
        //TODO: Add some default Offers to a SmartEntity
        public SmartEntity(FractionalHex hexCoords, float speed) : base(hexCoords, [])
        {
            Assert.True(speed >= 0);

            HexCoords = hexCoords;
            Speed = speed;
        }
    }
}

using LaYumba.Functional;
using MeanderingHeroes.Engine.Components;
using Xunit;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record Entity
    {
        public Hex Hex { get; init; }
        public FractionalHex HexCoords { get; init; }
        public int Id { get; private init; }
        internal Entity(int id, FractionalHex hexCoords)
        {
            Id = id;
            HexCoords = hexCoords;
            Hex = hexCoords.Round();
        }
        public override int GetHashCode() => Id;
    }
    public record SmartEntity : Entity
    {
        public Option<FractionalHex> Destination { get; init; } = None;
        public float Speed { get; init; } = 0F;
        //TODO: Add some default Offers to a SmartEntity
        internal SmartEntity(int id, FractionalHex hexCoords, float speed) : base(id, hexCoords)
        {
            Assert.True(speed >= 0);

            HexCoords = hexCoords;
            Speed = speed;
        }
    }
}

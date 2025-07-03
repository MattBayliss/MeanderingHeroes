using LaYumba.Functional;
using MeanderingHeroes.Engine.Components;
using Xunit;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public readonly record struct Entity
    {
        public FractionalHex HexCoords { get; init; }
        public int Id { get; private init; }
        public float Speed { get; init; } = 0F;
        /// <summary>
        /// General heartiness - decreases due to starvation, illness, poison. Death occurs at 0
        /// </summary>
        public Utility Constitution { get; init; } = 1F;
        public Utility Hunger { get; init; } = 0F;
        public float FoodSupply { get; init; } = 0F;
        internal Entity(int id, FractionalHex hexCoords, float speed)
        {
            Assert.True(speed >= 0);
            Id = id;
            HexCoords = hexCoords;
            Speed = speed;
        }
        public override int GetHashCode() => Id;
    }
}

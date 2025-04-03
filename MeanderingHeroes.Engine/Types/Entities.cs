using MeanderingHeroes.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MeanderingHeroes.Engine.Types
{
    public record Entity
    {
        public FractionalHex HexCoords { get; init; }
        /// <summary>
        /// The fraction of hex width covered per tick, assuming a terrain cost of 1.0 (i.e. speed of 1.0f means 1 hex / tick)
        /// </summary>
        public float Speed { get; init; } = 0F;
        public IImmutableList<IConsideration> Considerations { get; init; } = [];
        public Entity(FractionalHex axialCoords, float speed)
        {
            Assert.True(speed >= 0);

            HexCoords = axialCoords;
            Speed = speed;
        }
    }
}

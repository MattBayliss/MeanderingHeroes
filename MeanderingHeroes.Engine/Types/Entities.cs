using MeanderingHeroes.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MeanderingHeroes.Engine.Types
{
    public record Entity
    {
        public FractionalHex AxialCoords { get; init; }
        public float Speed { get; init; } = 0F;
        public IImmutableList<IConsideration> Considerations { get; init; } = [];
        public Entity(FractionalHex axialCoords, float speed)
        {
            Assert.True(speed >= 0);

            AxialCoords = axialCoords;
            Speed = speed;
        }
    }
}

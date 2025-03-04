using MeanderingHeroes.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MeanderingHeroes.Types
{
    public record Entity
    {
        public Point Location { get; init; }
        public float Speed { get; init; } = 0F;
        public IImmutableList<IConsideration> Considerations { get; init; } = [];
        public Entity(Point location, float speed)
        {
            Assert.True(speed >= 0);

            Location = location;
            Speed = speed;
        }
    }
}

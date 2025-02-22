using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MeanderingHeroes.Types
{
    public abstract record Entity(Point Location)
    {
        public float Speed { get; init; } = 0F;
        public IImmutableList<Goal> Goals { get; init; } = [];
        public abstract Entity Update();
    }

    public record Hero : Entity
    {
        public Hero(Point location, float speed) : base(location)
        {
            Assert.True(speed >= 0);

            Location = location;
            Speed = speed;
        }
        public override Hero Update()
        {
            return Goals.Aggregate(this, (hero, goal) => goal.Update(hero));
        }
    }
}

using LaYumba.Functional;
using MeanderingHeroes.Types;
using System.Collections.Immutable;


namespace MeanderingHeroes.Test
{
    public class MoveTests
    {
        [Fact]
        public void OneTickTest()
        {
            var startAt = new Location(0F, 0F);
            var destination = new Location(0F, 10F);
            var speed = 2F;

            var hero = new Hero(startAt, speed).AddGoal(new MoveGoal(destination));
            Assert.Equal(startAt, hero.Location);

            hero = hero.Update();

            Assert.Equal(startAt with { Y = startAt.Y + speed }, hero.Location);
        }
    }
}
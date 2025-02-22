using MeanderingHeroes.Functions;
using MeanderingHeroes.Types;

namespace MeanderingHeroes.Test
{
    public class MoveTests
    {
        [Fact]
        public void OneTickTest()
        {
            var grid = new Grid(10, 10);

            var hexStart = new Hex(0, 0);
            var hexDestination = new Hex(3, 0); // third hex to the east

            // in cartesian
            var startAt = hexStart.Centre();
            var endAt = hexDestination.Centre();

            Assert.Equal(3f * Functions.Map.UnitsPerHex, endAt.X - startAt.X, 0.01f);
            Assert.Equal(0f, endAt.Y);
            Assert.Equal(0f, startAt.Y);

            // speed relative to hex centres
            var speed = 1F;

            var hero = new Hero(startAt, speed).AddGoal(new MoveGoal(endAt));
            Assert.Equal(startAt, hero.Location);

            hero = hero.Update();

            Assert.Equal(startAt with { X = startAt.X + speed }, hero.Location);
        }
    }
}
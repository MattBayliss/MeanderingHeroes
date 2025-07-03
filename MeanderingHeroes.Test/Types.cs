using MeanderingHeroes.Engine.Types;

namespace MeanderingHeroes.Test
{
    public class Types
    {
        [Fact]
        public void UtilityMustBeBetween0and1()
        {
            Utility minus0point1 = -0.1f;
            Assert.Equal<float>(0f, minus0point1);
            Utility over1 = 1.1f;
            Assert.Equal<float>(1f, over1);

            var randomFloatBetween0And1 = Random.Shared.NextSingle();

            Utility validUtility = randomFloatBetween0And1;
            Assert.Equal(randomFloatBetween0And1, (float)validUtility);
        }
        [Fact]
        public void UtilityMathsWorks()
        {
            Utility first = 0.3f;
            Utility second = 0.4f;

            Assert.Equal(0.1f, (float)second - (float)first, 0.0001f);
            Assert.Equal(0.1f, second - first, 0.0001f);

            // clamped between 0 and 1
            Assert.Equal(0f, first - second, 0.0001f);
            Assert.Equal(1f, first + second + second, 0.0001f);

            Assert.Equal(0.12f, first * second, 0.0001f);
            Assert.Equal(0.3f / 0.4f, first / second, 0.0001f);
            Assert.True(first < second);
        }
    }
}

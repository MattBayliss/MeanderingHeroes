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
    }
}

using MeanderingHeroes.Engine.Components;

namespace MeanderingHeroes.Engine.Types
{
    public static class CurvesLibrary
    {
        public static Curve Linear(Utility minUtility, Utility maxUtility, float gradient) 
            => input => float.Max(float.Min(maxUtility, input * gradient), minUtility);
        /// <summary>
        /// A default Curve for Distance - guessing - but maybe 0.7 utility for 2 hexes away as a guide?
        /// </summary>
        public static Curve Distance => InverseSquared(2.8f);
        public static Curve InverseSquared(float factor) => input => input == 0 ? 1f : Utility.Clamp(factor / (input * input));
    }
}

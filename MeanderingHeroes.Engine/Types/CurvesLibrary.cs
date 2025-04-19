using MeanderingHeroes.Engine.Components;

namespace MeanderingHeroes.Engine.Types
{
    public static class CurvesLibrary
    {
        public static Curve Linear(Utility minUtility, Utility maxUtility, float gradient) 
            => input => float.Max(float.Min(maxUtility, input * gradient), minUtility);
    }
}

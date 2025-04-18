using LaYumba.Functional;
using MeanderingHeroes.Engine.Components;
using MeanderingHeroes.Engine.Types;
using MeanderingHeroes.Engine.Types.AI;
using System.Numerics;
using Xunit;

namespace MeanderingHeroes.Engine
{
    public static partial class Functions
    {        
        public static bool WithinMargin(this Vector2 a, Vector2 b, float margin) => Vector2.Subtract(a, b).LengthSquared() < MathF.Pow(margin, 2);

        public static IEnumerable<Hex> Neighbours(this Hex current)
            => Hex.Directions.Select(dir => current + dir);
    }
}

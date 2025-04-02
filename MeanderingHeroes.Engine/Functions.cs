using LaYumba.Functional;
using MeanderingHeroes.Engine.Components;
using MeanderingHeroes.Engine.Types;
using System.Numerics;
using Xunit;

namespace MeanderingHeroes.Engine
{
    public static partial class Functions
    {        
        public static Hex ToHex(this Vector2 v) => Transforms.Instance.ToAxial(v).Round();

        public static Vector2 Centre(this Hex hex) => Transforms.Instance.ToVector2(hex);
        public static Vector2 ToVector2(this FractionalHex fhex) => Transforms.Instance.ToVector2(fhex);

        public static bool WithinMargin(this Vector2 a, Vector2 b, float margin) => Vector2.Subtract(a, b).LengthSquared() < MathF.Pow(margin, 2);

        public static IEnumerable<Hex> Neighbours(this Hex current) 
            => Hex.Directions.Select(dir => current + dir);
    }

    public static partial class Core
    {
        public static Entity AddConsideration(this Entity entity, IConsideration consideration) 
            => entity with { 
                Considerations = entity.Considerations.Add(consideration)
            };
    }
}

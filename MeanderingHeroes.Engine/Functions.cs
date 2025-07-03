using LaYumba.Functional;
using MeanderingHeroes.Engine.Components;
using MeanderingHeroes.Engine.Types;
using System.Numerics;
using Xunit;

namespace MeanderingHeroes.Engine
{
    public static partial class Functions
    {
        public static bool WithinMargin(this Vector2 a, Vector2 b, float margin) => Vector2.Subtract(a, b).LengthSquared() < MathF.Pow(margin, 2);
        public static bool EqualsWithMargin(this float @this, float other, float margin) => MathF.Abs(@this - other) <= margin;

        public static IEnumerable<Hex> Neighbours(this Hex current)
            => Hex.Directions.Select(dir => current + dir);
        public static IEnumerable<Hex> Neighbours(this FractionalHex current) 
            => current.Round().Pipe(hex => Hex.Directions.Select(dir => hex + dir));

        public static IEnumerable<TItem> AppendIfSome<TItem>(this IEnumerable<TItem> @this, Option<TItem> value) => @this.Concat(value.AsEnumerable());
    }
}

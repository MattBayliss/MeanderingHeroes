using LaYumba.Functional;
using MeanderingHeroes.Engine.Components;
using MeanderingHeroes.Engine.Types;
using System.Numerics;
using Xunit;

namespace MeanderingHeroes.Engine
{
    public static partial class Functions
    {
        private readonly static float Sqrt3 = MathF.Sqrt(3);
        // hardcoding the scale to 1.0 as shortest distance between two hex centres.
        // it's up to the interface to transform points to the local scale
        private static float _hexWidth = 1f;
        private static float _size = _hexWidth / Sqrt3;
        /// <summary>
        /// shortest distance between two hex centres
        /// https://www.redblobgames.com/grids/hexagons/#basics
        /// </summary>
        public static Hex ToHex(this Point p)
        {
            float q = (p.X * Sqrt3 / 3f - p.Y / 3f) / _size;
            float r = (p.Y * 2f / 3f) / _size;

            return (new FractionalHex(q, r)).Round();
        }
        
        public static Point Centre(this Hex hex)
            => new Point(
                // X: _size * (Sqrt3 * hex.Q + (Sqrt3 / 2f) * hex.R),
                X: _hexWidth * (hex.Q + hex.R / 2f),
                Y: _size * 1.5f * hex.R
            );

        public static bool WithinMargin(this Point a, Point b, float margin)
        {
            Assert.True(margin > 0);
            return MathF.Abs(a.X - b.X) + MathF.Abs(a.Y - b.Y) < margin;
        }

        public static IEnumerable<Hex> Neighbours(this Hex current) 
            => Hex.Directions.Select(dir => current + dir);
       
        public static Point Sum(this IEnumerable<Point> @this) => @this.Aggregate(new Vector2(0f, 0f), (sum, point) => Vector2.Add(sum, point));
    }

    public static partial class Core
    {
        public static Entity AddConsideration(this Entity entity, IConsideration consideration) 
            => entity with { 
                Considerations = entity.Considerations.Add(consideration)
            };
    }
}

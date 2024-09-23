using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MeanderingHeroes
{
    public static partial class ModelLibrary
    {
        public delegate Turn<Location> NextWaypoint(Map map, Hero hero);

        public static NextWaypoint StraightLinePath(float speed, [NotNull]Location destination)
        {
            if(speed <= 0)
            {
                throw new ArgumentOutOfRangeException("speed", "speed must be > 0");
            }

            return (Map map, Hero hero) =>
            {
                var targetVector = Vector2.Subtract(destination, hero.Location);
                var calcTravelVector = (float mag) => targetVector.Unit().SetMagnitude(mag) + hero.Location;
                return (targetVector.Length() < speed)
                    ? Done(destination)
                    : Turn((Location)calcTravelVector(speed));
            };
        }
    }
}

using MeanderingHeroes.Types.Doers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MeanderingHeroes
{
    public static partial class ModelLibrary
    {
        public delegate Turn<Location> NextWaypoint(Map map, Doer doer);

        public static NextWaypoint StraightLinePath(float speed, [NotNull]Location destination)
        {
            if(speed <= 0)
            {
                throw new ArgumentOutOfRangeException("speed", "speed must be > 0");
            }

            // map is currently unused - but will be used to calculate terrain costs at some point
            return (map, doer) =>
            {
                var targetVector = Vector2.Subtract(destination, doer.Location);
                var calcTravelVector = (float mag) => targetVector.Unit().SetMagnitude(mag) + doer.Location;
                return (targetVector.Length() < speed)
                    ? Done(destination)
                    : Turn((Location)calcTravelVector(speed));
            };
        }
    }
}

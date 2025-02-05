using MeanderingHeroes.Types;
using MeanderingHeroes.Types.Doers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MeanderingHeroes
{
    public static partial class ModelLibrary
    {
        public delegate Turn<HexCoordinates> NextWaypoint(Graph map, Doer doer);

        public static NextWaypoint FastestPath(MovementType movementType, HexCoordinates destination)
        {
            return (map, doer) =>
            {
                HexCoordinates nextHex = map.GetShortestPath(doer.Location, (Coordinate3D)destination, movementType).FirstOrDefault(destination);

                return (nextHex == destination)
                    ? Done(destination)
                    : Turn((HexCoordinates)nextHex);
            };
        }
    }
}

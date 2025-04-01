using LaYumba.Functional;
using MeanderingHeroes.Engine.Components;
using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine
{
    public static class PathFinding
    {
        public static StatefulConsideration<ImmutableList<Hex>> GeneratePathGoalConsideration(Grid grid, Hex start, Hex end)
        {
            FractionalHex endHex = end;

            // function to use when we're in a hex in the path and we want to steer to the next one
            Func<FractionalHex, FractionalHex, FractionalHex> veeredDestination = (hex1, hex2) => (FractionalHex)Vector3.Divide(Vector3.Add(hex1, hex2), 2);

            // function for checking if we're at the first hex of the path, and if so, move to next hex
            Func<ImmutableList<Hex>, Entity, (ImmutableList<Hex> path, Entity entity)> moveAlongPath = (path, entity) =>
            {
                var entityhex = entity.AxialCoords.Round();

                (bool atFirstHex, FractionalHex destination) = path switch
                {
                    // no more path, assume the hex centre is the destination
                    []                          => (false, entityhex),

                    // we're at the final hex, clear the remaining path, and make the hex centre the destination
                    [var p1]
                        when p1 == entityhex    => (true, entityhex),

                    // one hex to go on the path, but we're not there yet. Mark that hex as the destination
                    [var p1]                    => (false, p1),

                    // two or more hexes left on the path, and we've made it to the next hex,
                    // remove the current hex from the path, and start veering towards next hex
                    [var p1, var p2, ..]
                        when p1 == entityhex    => (true, veeredDestination(p1, p2)),

                    // two or more hexes left on the path, and we're not at the first hex yet -
                    // mark it as the destination
                    [var p1, var p2, ..]        => (false, p1)
                };

                var vectorToDestination = Vector3.Subtract(destination, entity.AxialCoords);
                var distanceToDestination = vectorToDestination.Length();

                Func<FractionalHex> calcNextCoords = () =>
                {
                    var distanceCovered = entity.Speed / grid.Terrain[entityhex].MovementCost;
                    return distanceCovered < distanceToDestination
                        ? (FractionalHex)(entity.AxialCoords + Vector3.Multiply(vectorToDestination, distanceCovered / distanceToDestination))
                        : destination;
                };

                FractionalHex nextCoords = distanceToDestination < 0.001f ? destination : calcNextCoords();

                return (atFirstHex ? path.Skip(1).ToImmutableList() : path, entity with { AxialCoords = nextCoords });

            };

            return new StatefulConsideration<ImmutableList<Hex>>(
                c11nState: grid.AStarPath(start, end).ToImmutableList(),
                // hardcoded for now
                utilityFunc: (_, _, entity) => entity.AxialCoords == endHex ? 0 : 0.3f,
                updateFunc: (path, entity) => moveAlongPath(path, entity),
                toRemove: (entity) => entity.AxialCoords == endHex
            );
        }
        // mostly copied line for line from https://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
        public static IEnumerable<Hex> AStarPath(this Grid grid, Hex start, Hex end)
        {
            if(!grid.InBounds(start) || !grid.InBounds(end))
            {
                return [];
            }
            var cameFrom = new Dictionary<Hex, Hex> { { start, start } };
            var costSoFar = new Dictionary<Hex, double> { { start, 0 } };

            // kind of want to implement my own functional PriorityQueue - but finish my sundae first
            var frontier = new PriorityQueue<Hex, double>();
            frontier.Enqueue(start, 0);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current == end)
                {
                    break;
                }
                current.Neighbours()
                    .Where(nhex => nhex.Q >= 0 && nhex.R >= 0)
                    .Where(grid.InBounds)
                    .ForEach(next =>
                {
                    var newCost = costSoFar[current] + grid.Terrain[next].MovementCost;

                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar.AddOrUpdate(next, newCost);
                        var priority = newCost + end.Distance(next);
                        frontier.Enqueue(next, priority);
                        cameFrom.AddOrUpdate(next, current);
                    }
                });
            }
            // now unpack
            List<Hex> result = [];

            var hexout = end;
            while (hexout != start)
            {
                result.Insert(0, hexout);
                hexout = cameFrom[hexout];
            }

            return result;
        }
    }
}

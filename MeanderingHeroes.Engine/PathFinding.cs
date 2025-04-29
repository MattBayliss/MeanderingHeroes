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
        private static float Sqrt2 = MathF.Sqrt(2);

        public static StatefulBehaviour<ImmutableList<Hex>> GeneratePathGoalBehaviour(Game game, InteractionBase interaction, Hex end)
        {
            FractionalHex endHex = end;

            // function to use when we're in a hex in the path and we want to steer to the next one
            Func<FractionalHex, FractionalHex, FractionalHex> veeredDestination = (hex1, hex2) => (FractionalHex)Vector3.Divide(Vector3.Add((Vector3)hex1, (Vector3)hex2), 2);

            // function for checking if we're at the first hex of the path, and if so, move to next hex
            Func<ImmutableList<Hex>, SmartEntity, (ImmutableList<Hex> path, SmartEntity entity)> moveAlongPath = (path, entity) =>
            {
                var entityhex = entity.HexCoords.Round();

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

                var vectorToDestination = Vector3.Subtract((Vector3)destination, (Vector3)entity.HexCoords);
                var distanceToDestination = vectorToDestination.Length();

                Func<FractionalHex> calcNextCoords = () =>
                {
                    // entity speed is defined as fraction of a hex travelled per tick, assuming travel cost of 1.0,
                    // but the axial vector of (q:1, r:0) is actually (q:1, r:0:, s:-1) and so has a length of √2,
                    // so speed = entity.Speed * √2
                    var distanceCovered = entity.Speed * Sqrt2 / game.HexMap.Terrain[entityhex].MovementCost;
                    return distanceCovered < distanceToDestination
                        ? (FractionalHex)((Vector3)entity.HexCoords + (vectorToDestination * distanceCovered / distanceToDestination))
                        : destination;
                };

                FractionalHex nextCoords = distanceToDestination < 0.001f ? destination : calcNextCoords();

                return (atFirstHex ? path.Skip(1).ToImmutableList() : path, entity with { HexCoords = nextCoords });

            };

            return new StatefulBehaviour<ImmutableList<Hex>>(
                name: "Path-Finding",
                initState: entity => PathFinding.AStarPath(game.HexMap, entity.Hex, end).ToImmutableList(),
                interaction: interaction,
                updateFunc: (game, path, entity) => moveAlongPath(path, entity),
                toRemove: (entity) => entity.HexCoords == endHex
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

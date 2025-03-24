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
            Point endPoint = end.Centre();

            // function for checking if we're at the first hex of the path, and if so, move to next hex
            Func<ImmutableList<Hex>, Entity, (ImmutableList<Hex> path, Entity entity)> moveAlongPath = (path, entity) =>
            {
                var entityhex = entity.Location.ToHex();

                (bool atFirstHex, Point destination) = path switch
                {
                    // no more path, assume the hex centre is the destination
                    []                          => (false, entityhex.Centre()),

                    // we're at the final hex, clear the remaining path, and make the hex centre the destination
                    [var p1]
                        when p1 == entityhex    => (true, entityhex.Centre()),

                    // one hex to go on the path, but we're not there yet. Mark that hex as the destination
                    [var p1]                    => (false, p1.Centre()),

                    // two or more hexes left on the path, and we've made it to the next hex,
                    // remove the current hex from the path, and start veering towards next hex
                    [var p1, var p2, ..]
                        when p1 == entityhex    => (true, (Point)Vector2.Divide(Vector2.Add(p1.Centre(), p2.Centre()), 2)),

                    // two or more hexes left on the path, and we're not at the first hex yet -
                    // mark it as the destination
                    [var p1, var p2, ..]        => (false, p1.Centre())
                };

                // TODO: don't calculate distanceCovered if we're already at destination

                var vectorToDestination = Vector2.Subtract(destination, entity.Location);
                var distanceToDestination = vectorToDestination.Length();

                // Assuming entityhex exists on the grid - it should do!
                // The grid's hexes were used to make the path, and even
                // if the entity starts off the grid, the path would be empty
                var distanceCovered = entity.Speed / grid.Terrain[entityhex].MovementCost;

                Point nextPoint = distanceToDestination > distanceCovered
                    ? entity.Location + Vector2.Multiply(
                        vectorToDestination,
                        distanceCovered / distanceToDestination)
                    : destination;



                return (atFirstHex ? path.Skip(1).ToImmutableList() : path, entity with { Location = nextPoint });

            };
            return new StatefulConsideration<ImmutableList<Hex>>(
                c11nState: grid.AStarPath(start, end).ToImmutableList(),
                // hardcoded for now
                utilityFunc: (_, _, _, entity) => entity.Location == endPoint ? 0 : 0.3f,
                updateFunc: (path, entity) => moveAlongPath(path, entity),
                toRemove: (entity) => entity.Location == endPoint
            );
        }
        // mostly copied line for line from https://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
        public static IEnumerable<Hex> AStarPath(this Grid grid, Hex start, Hex end)
        {
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

        public static Func<Entity, Entity> GenerateDestinationUpdater(Point destination) => entity =>
        {
            if (entity.Speed <= 0) return entity;

            var vectorToDestination = Vector2.Subtract(destination, entity.Location);
            var distanceToDestination = vectorToDestination.Length();

            Point nextPoint = distanceToDestination > entity.Speed
                ? entity.Location + Vector2.Multiply(
                    vectorToDestination,
                    entity.Speed / distanceToDestination)
                : destination;

            return entity with { Location = nextPoint };
        };
    }
}

using LaYumba.Functional;
using MeanderingHeroes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes
{
    public static class PathFinding
    {
        // mostly copied line for line from https://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
        public static IEnumerable<Hex> AStarPath(this Grid grid, Hex start, Hex end)
        {
            var cameFrom = new Dictionary<Hex, Hex> { { start, start } };
            var costSoFar = new Dictionary<Hex, double> { {start, 0} };

            // kind of want to implement my own functional PriorityQueue - but finish my sundae first
            var frontier = new PriorityQueue<Hex, double>();
            frontier.Enqueue(start, 0);

            while(frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if(current == end)
                {
                    break;
                }
                current.Neighbours()
                    .Where(nhex => nhex.Q >= 0 && nhex.R >= 0)
                    .Where(nhex => nhex.Q < grid.Width && nhex.R < grid.Height)
                    .ForEach(next =>
                {
                    var newCost = costSoFar[current] + grid.Terrain[next.Q, next.R].MovementCost;

                    if (!costSoFar.ContainsKey(next) || (newCost < costSoFar[next]))
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
            while(hexout != start)
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

            Point nextPoint = (distanceToDestination > entity.Speed)
                ? entity.Location + Vector2.Multiply(
                    vectorToDestination,
                    entity.Speed / distanceToDestination)
                : destination;

            return entity with { Location = nextPoint };
        };
    }
}

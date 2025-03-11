using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record Goal
    {
        public abstract T Update<T>(T entity) where T : Entity;
    }

    public record MoveGoal : Goal
    {
        public Point Destination { get; init; }
        public MoveGoal(Point destination)
        {
            Destination = destination;
        }

        public override T Update<T>(T entity)
        {
            if (entity.Speed <= 0) return entity;

            var vectorToDestination = Vector2.Subtract(Destination, entity.Location);
            var distanceToDestination = vectorToDestination.Length();

            Point nextPoint = distanceToDestination > entity.Speed
                ? entity.Location + Vector2.Multiply(
                    vectorToDestination,
                    entity.Speed / distanceToDestination)
                : Destination;

            return entity with { Location = nextPoint };
        }
    }
}

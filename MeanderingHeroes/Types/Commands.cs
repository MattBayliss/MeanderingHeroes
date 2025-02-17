using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types
{
    public abstract record Goal
    {
        public abstract T Update<T>(T entity) where T : Entity;
    }

    public record MoveGoal : Goal
    {
        public Location Destination { get; init; }
        public MoveGoal(Location destination)
        {
            Destination = destination;
        }

        public override T Update<T>(T entity)
        {
            return entity switch
            {
                { Speed: <= 0 } => entity,
                { Speed: var s, Location: var loc } => entity with 
                    { 
                        Location = Vector2.Multiply(Vector2.Normalize(Vector2.Subtract(Destination, loc)), s) 
                    }
            };
        }
    }
}

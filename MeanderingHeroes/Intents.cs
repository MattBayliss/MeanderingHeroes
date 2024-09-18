using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes
{
    public abstract class Intent;
    public abstract class HeroIntent : Intent
    {
        public abstract Func<GameState, Hero, Hero> HeroComputation { get; init; }
    }

    public class MoveIntent : HeroIntent
    {
        // 💡path-finding
        //      `Func<GameState, Hero, IEnumerable<Location>> DetermineWaypoints` 
        public Location Destination { get; init; }
        public override Func<GameState, Hero, Hero> HeroComputation { get; init; }

        public MoveIntent([NotNull] Location destination)
        {
            Destination = destination;
            HeroComputation = (state, hero) =>
                CalculateVector(state, hero) switch
                {
                    { reachedDestination: true, vector: var v }
                        => hero with
                        {
                            Location = v,
                            // need to be able to identify the intent to remove - guid?
                            Intents = hero.Intents.Remove(this) 
                        },
                    { vector: var v } => hero with { Location = v }
                };
        }

        private (Vector2 vector, bool reachedDestination) CalculateVector(GameState state, Hero hero)
        {
            var speed = 2.0f;
            var targetVector = Vector2.Subtract(Destination, hero.Location);
            var calcTravelVector = (float mag) => targetVector.Unit().SetMagnitude(mag) + hero.Location;
            return (targetVector.Length() < speed)
                ? (Destination, true)
                : (calcTravelVector(speed), false);
        }
    }

}

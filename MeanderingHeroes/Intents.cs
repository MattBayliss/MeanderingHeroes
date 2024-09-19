using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MeanderingHeroes
{

    public class MoveIntent : HeroIntent
    {
        // 💡path-finding
        //      `Func<GameState, Hero, IEnumerable<Location>> DetermineWaypoints` 
        public Location Destination { get; init; }
        public override Func<GameState, Hero, (Hero, Events)> HeroComputation { get; init; }

        public static MoveIntent Create(Hero hero, [NotNull] Location destination) =>
            new MoveIntent(hero.Id, destination);

        private MoveIntent(int heroId, Location destination) : base(heroId)
        {
            Destination = destination;
            HeroComputation = (state, hero) =>
                CalculateVector(state, hero) switch
                {
                    { done: true, vector: var v }
                        => (hero with { Location = v }, ImmutableList.Create<Event>(new EndEvent(HeroId, this))),
                    { vector: var v } => (hero with { Location = v }, ImmutableList<Event>.Empty)
                };
        }

        private EndEvent ArrivedAtDestination() => new EndEvent(HeroId, this);

        private (Vector2 vector, bool done) CalculateVector(GameState state, Hero hero)
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

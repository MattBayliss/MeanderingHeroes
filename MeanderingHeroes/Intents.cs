using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static MeanderingHeroes.ModelLibrary;

namespace MeanderingHeroes
{

    public class MoveIntent : HeroIntent
    {
        private NextWaypoint _nextWaypoint { get; init; }
        /// <summary>
        /// A stateful computation function - takes the GameState and the Hero state, and
        /// produces a tuple of a new Hero state, and any Events that were triggered
        /// </summary>
        public override Func<GameState, Hero, (Hero, Events)> HeroComputation { get; init; }

        public static MoveIntent Create(Hero hero, NextWaypoint nextWaypoint) =>
            new MoveIntent(hero.Id, nextWaypoint);

        private MoveIntent(int heroId, NextWaypoint nextWaypoint) : base(heroId)
        {
            _nextWaypoint = nextWaypoint;
            HeroComputation = (state, hero) =>
                nextWaypoint(state.Map, hero) switch
                {
                    Done<Location> d => (hero with { Location = d }, ImmutableList.Create<Event>(ArrivedAtDestination)),
                    Turn<Location> next 
                        => (hero with { Location = next }, ImmutableList.Create<Event>(new ArrivedEvent(HeroId, next)))
                };
        }
        private EndEvent ArrivedAtDestination => new EndEvent(HeroId, this);
    }
}
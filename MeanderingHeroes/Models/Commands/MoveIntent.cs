using MeanderingHeroes.Models.Doers;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static MeanderingHeroes.ModelLibrary;

namespace MeanderingHeroes.Models.Commands
{
    public class FleeDangerIntent : DoerIntent
    {
        public FleeDangerIntent(int id) : base(id) { }
    }
    public class MoveIntent : DoerIntent
    {
        private NextWaypoint _nextWaypoint { get; init; }
        /// <summary>
        /// A stateful computation function - takes the GameState and the Doer state, and
        /// produces a tuple of a new Doer state, and any Events that were triggered
        /// </summary>
        public override Func<GameState, Doer, (Doer Doer, Events Events)> ProcessIntent { get; init; }

        public static MoveIntent Create(Hero hero, NextWaypoint nextWaypoint) =>
            new MoveIntent(hero.Id, nextWaypoint);

        private MoveIntent(int heroId, NextWaypoint nextWaypoint) : base(heroId)
        {
            _nextWaypoint = nextWaypoint;
            ProcessIntent = (state, hero) =>
                nextWaypoint(state.Map, hero) switch
                {
                    Done<Location> d => (hero with { Location = d }, ImmutableList.Create<Event>(ArrivedAtDestination)),
                    Turn<Location> next
                        => (hero with { Location = next }, ImmutableList.Create<Event>(ArrivedEvent.Create(DoerId, next)))
                };
        }
        private EndEvent ArrivedAtDestination => EndEvent.Create(DoerId, this);
    }
    public static partial class ModelLibrary
    {
        public static Hero AddMoveIntent(this Hero hero, NextWaypoint pathFinder) =>
            hero with { Intents = hero.Intents.Add(MoveIntent.Create(hero, pathFinder)) };
    }
}
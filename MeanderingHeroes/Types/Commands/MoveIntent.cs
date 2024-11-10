using LaYumba.Functional;
using MeanderingHeroes.Types.Doers;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using static MeanderingHeroes.ModelLibrary;

namespace MeanderingHeroes.Types.Commands
{
    public record FleeDangerIntent : DoerIntent
    {
        public FleeDangerIntent(int id) : base(id) { }
    }
    public record MoveIntent : DoerIntent
    {
        private NextWaypoint _nextWaypoint { get; init; }

        public static MoveIntent Create(Hero hero, NextWaypoint nextWaypoint) =>
            new MoveIntent(hero.Id, nextWaypoint);

        private MoveIntent(int heroId, NextWaypoint nextWaypoint) : base(heroId)
        {
            _nextWaypoint = nextWaypoint;

            var updateHero = (GameState state, Hero hero, Location locn)
                => state with { Doers = state.Doers.Replace(hero, hero with { Location = locn }) };

            Func<Map, Func<Hero, (Hero hero, Location location, Events events)>> nextWaypointResult =
                map => hero => _nextWaypoint(map, hero) switch
                    {
                        Done<Location> d
                            => (hero, d, ImmutableList.Create<Event>(ArrivedAtDestination)),
                        Turn<Location> next
                            => (hero, next, ImmutableList.Create<Event>(ArrivedEvent.Create(DoerId, next)))
                    };

            ProcessIntent = (GameState state) => state.GetDoer<Hero>(heroId)
                .Map(nextWaypointResult(state.Map))
                .Match(
                    None: () => (state, []),
                    Some: hle => (updateHero(state, hle.hero, hle.location), hle.events)
                );
        }
        private EndEvent ArrivedAtDestination => EndEvent.Create(this);
    }
    public static partial class ModelLibrary
    {
        public static GameState AddMoveIntent(this GameState state, Hero hero, NextWaypoint pathFinder) =>
            state with { Commands = state.Commands.Add(MoveIntent.Create(hero, pathFinder)) };
    }
}
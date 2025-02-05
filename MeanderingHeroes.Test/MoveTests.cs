using LaYumba.Functional;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using MeanderingHeroes.Types.Commands;

using Location = System.Numerics.Vector2;

using static MeanderingHeroes.ModelLibrary;
using static MeanderingHeroes.Test.Helpers;
using static LaYumba.Functional.F;
using MeanderingHeroes.Types.Doers;
using MeanderingHeroes.Types;
using HexCore;

namespace MeanderingHeroes.Test
{
    public class MoveTests
    {
        [Fact]
        public void OneTickTest()
        {
            var hero = new Hero("bob", new HexCoordinates(0, 0));

            var state = new GameState(MakeHexMap(10, 10, (_,_) => Constants.GrassTerrain));
            Assert.Empty(state.Doers);
            state = state.Add(hero);
            Assert.Single(state.Doers);
            var destination = new HexCoordinates(1, 0);
            state = state.AddMoveIntent(hero, FastestPath(Constants.Walking, destination));

            Assert.Single(state.Commands);
            var moveIntent = Assert.IsType<MoveIntent>(state.Commands.Single());

            var (postTurnState, events) = GameRunner.RunTurn(state);

            var updatedDoer = Assert.Single(postTurnState.Doers);
            var updatedHero = Assert.IsType<Hero>(updatedDoer);

            var optionheroByGet = postTurnState.GetDoer<Hero>(hero.Id);
            var heroByGet = AssertIsSome(optionheroByGet);

            Assert.Equal(updatedHero, heroByGet);

            Assert.Equal(destination, heroByGet.Location);
            
            Assert.Equal(2, events.Count);
            var movedEvent = Assert.IsType<ArrivedEvent>(events.First());
            Assert.Equal(heroByGet.Id, movedEvent.DoerId);
            Assert.Equal(heroByGet.Location, movedEvent.Location);

        }
        [Fact]
        public void MoveEast()
        {
            // make a map 10 tiles wide, 3 tiles high
            var map = MakeHexMap(10, 3, (x, y) => Constants.GrassTerrain);

            var start = new HexCoordinates(0, 1);
            var finish = new HexCoordinates(9, 1);

            // make a hero that starts at the west/left-most tile, middle row
            var hero = new Hero("Testo", start);

            var speed2StraightLinePath = FastestPath(Constants.Walking, finish);

            var initialState = new GameState(map)
                .Add(hero)
                .AddMoveIntent(hero, speed2StraightLinePath);

            var addedIntent = Assert.Single(initialState.Commands);
            var moveIntent = Assert.IsType<MoveIntent>(addedIntent);

            // actual test, full of bad side-effects and whatnot
            int turnLimit = 10;
            int attempt = 0;

            ImmutableList<Event> events = [];

            GameState state = initialState;

            Range(1, turnLimit).ForEach(
                _ =>
                {
                    var gameevents = GameRunner.RunTurn(state);
                    events = events.AddRange(gameevents.Events);
                    state = gameevents.State;
                    attempt++;
                }
            );
            var endEvent = Assert.IsType<EndEvent>(events.Last());
            var endEventIntent = Assert.IsType<MoveIntent>(endEvent.Intent);

            Assert.Equal(hero.Id, endEventIntent.DoerId);

            var arrivedEvents = events.OfType<ArrivedEvent>().ToArray();
            Assert.NotEmpty(arrivedEvents);
            var lastArrivedEvent = events.OfType<ArrivedEvent>().Last();

            Assert.Equal(finish, lastArrivedEvent.Location);
            Assert.Equal(hero.Id, lastArrivedEvent.DoerId);

            // every turn the hero should have moved a distance equal to speed,
            // except for the last turn, which will be the remainder
            var arrivedCount = arrivedEvents.Length;

            var turnLocations = arrivedEvents
                .Select(ae => ae.Location)
                .Prepend(start)
                .ToArray();

            //Assert.Equal(distance, turnLocations.Last().Subtract(turnLocations.First()).Length());

            //for(int i = 0; i < turnLocations.Length - 2; i++)
            //{
            //    var loc1 = turnLocations[i];
            //    var loc2 = turnLocations[i + 1];
            //    Assert.Equal(speed, loc2.Subtract(loc1).Length());
            //}

            var finalhero = AssertIsSome(state.GetDoer<Hero>(hero.Id));
            Assert.Equal(finish, finalhero.Location);
            Assert.DoesNotContain(moveIntent, state.Commands);
        }
    }
}
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

namespace MeanderingHeroes.Test
{
    public class MoveTests
    {
        [Fact]
        public void OneTickTest()
        {
            var rnd = new Random();
            var distance = rnd.NextSingle() + 1f;
            var hero = new Hero("bob", new Location(0f, 0f));

            var state = new GameState(MakeMap(10, 10, (_,_) => Terrain.Grass));
            Assert.Empty(state.Doers);
            state = state.Add(hero);
            Assert.Single(state.Doers);
            state = state.AddMoveIntent(hero, StraightLinePath(distance, new Location(10f, 0f)));

            Assert.Single(state.Commands);
            var moveIntent = Assert.IsType<MoveIntent>(state.Commands.Single());

            var (postTurnState, events) = GameRunner.RunTurn(state);

            var updatedDoer = Assert.Single(postTurnState.Doers);
            var updatedHero = Assert.IsType<Hero>(updatedDoer);

            var optionheroByGet = postTurnState.GetDoer<Hero>(hero.Id);
            var heroByGet = AssertIsSome(optionheroByGet);

            Assert.Equal(updatedHero, heroByGet);

            Assert.Equal(distance, heroByGet.Location.X - hero.Location.X);
            Assert.Equal(hero.Location.Y, heroByGet.Location.Y);
            
            Assert.Single(events);
            var movedEvent = Assert.IsType<ArrivedEvent>(events.Single());
            Assert.Equal(heroByGet.Id, movedEvent.DoerId);
            Assert.Equal(heroByGet.Location, movedEvent.Location);

        }
        [Fact]
        public void MoveEast()
        {
            // make a map 10 tiles wide, 3 tiles high
            var map = MakeMap(10, 3, (x, y) => Terrain.Grass);

            var start = new Location(0f, 1f);
            var finish = new Location(9f, 1f);
            var distance = finish.Subtract(start).Length();
            var speed = 2.0f;

            // make a hero that starts at the west/left-most tile, middle row
            var hero = new Hero("Testo", start);

            var speed2StraightLinePath = StraightLinePath(speed, finish);


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
            Assert.True(events.Count > 2);
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

            Assert.Equal(distance, turnLocations.Last().Subtract(turnLocations.First()).Length());

            for(int i = 0; i < turnLocations.Length - 2; i++)
            {
                var loc1 = turnLocations[i];
                var loc2 = turnLocations[i + 1];
                Assert.Equal(speed, loc2.Subtract(loc1).Length());
            }

            var finalhero = AssertIsSome(state.GetDoer<Hero>(hero.Id));
            Assert.Equal(finish, finalhero.Location);
            Assert.DoesNotContain(moveIntent, state.Commands);
        }
    }
}
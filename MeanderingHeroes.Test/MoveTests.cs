using LaYumba.Functional;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

using Location = System.Numerics.Vector2;

using static MeanderingHeroes.ModelLibrary;
using static MeanderingHeroes.Test.Helpers;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Test
{
    public class MoveTests
    {
        [Fact]
        public void MoveEast()
        {
            // make a map 10 tiles wide, 3 tiles high
            var map = MakeMap(10, 3, (x,y) => Terrain.Grass);

            var start = new Location(0f, 1f);
            var finish = new Location(9f, 1f);
            var distance = finish.Subtract(start).Length();
            var speed = 2.0f;

            // make a hero that starts at the west/left-most tile, middle row
            var hero = new Hero(1, "Testo", start);

            var speed2StraightLinePath = StraightLinePath(speed, finish);

            // make an intent to move to the east/right most tile, middle row
            hero = hero.AddMoveIntent(speed2StraightLinePath);

            var addedIntent = Assert.Single(hero.Intents);

            var moveIntent = Assert.IsType<MoveIntent>(addedIntent);
            
            var initialState = new GameState(map).Add(hero);

            // actual test, full of bad side-effects and whatnot
            int turnLimit = 100;
            int attempt = 0;

            EndEvent? endEvent = null;
            var isTestHero = (Doer d) => d is Hero h ? h.Id == hero.Id : false;

            var testFunc = (GameState state, ImmutableList<Event> events) =>
            {
                attempt++;
                if (attempt > turnLimit) return false;

                var turnhero = state.Doers.FirstOrDefault(isTestHero);
                Assert.NotNull(turnhero);

                var distanceSoFar = turnhero.Location.Subtract(start).Length();

                var matchedEvent = events.FirstOrDefault(
                    ev => 
                        ev is EndEvent endev && endev.DoerId == hero.Id
                        && endev.Intent is MoveIntent mi
                );

                endEvent = (matchedEvent is EndEvent endev) ? endev : endEvent;

                var atEnd = endEvent != null;

                // should have travelled a distance equal to speed x turn number
                var expectedDistance = atEnd ? distance : speed * attempt;

                Assert.Equal(expectedDistance, distanceSoFar);
                
                return atEnd;
            };

            // one line to run the whole game, with the testFunc above controlling
            // when it stops
            var finalState = RunGame(initialState, testFunc);

            Assert.True(attempt < turnLimit);
            Assert.Equal(Math.Ceiling(distance / speed), attempt);
            Assert.NotNull(endEvent);
            Assert.Equal(hero.Id, endEvent.DoerId);
            var endedIntent = Assert.IsType<MoveIntent>(endEvent.Intent);

            var finalherostate = finalState.Doers.Find(h => h.Id == hero.Id);
            Assert.NotNull(finalherostate);
            var finalhero = Assert.IsType<Hero>(finalherostate);
            Assert.Equal(finish, finalhero.Location);
            Assert.DoesNotContain(moveIntent, finalhero.Intents);
        }
    }
}
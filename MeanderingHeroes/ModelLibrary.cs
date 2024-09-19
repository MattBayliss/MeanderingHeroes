using System;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;



namespace MeanderingHeroes
{
    using HerosIntent = (Hero Hero, HeroIntent Intent);

    // need to track:
    // map -> grid -> cell (weather, terrain, travel costs (n,s,e,w))
    // monsters -> monster (location, flags (hungry, scared, wounded, aggressive))
    // heroes -> hero (location, instructions, 
    public readonly record struct GameState(Map map, ImmutableList<Hero> Heroes);

    public static class ModelLibrary
    {
        public static GameState RunGame(GameState initialState, Func<GameState, Events, bool> until)
        {
            bool aborted = false;

            var stop = () => aborted;

            var finalTuple = IterateUntil(stop).Aggregate(
                (State: initialState, Events: ImmutableList<Event>.Empty),
                (stateevents, _) =>
                {
                    var nextTuple = stateevents.State.ActiveIntents()
                    .Aggregate(
                        (State: stateevents.State, Events: stateevents.Events),
                        (s, herointent) =>
                        {
                            var (h, events) = herointent.Intent.HeroComputation(s.State, herointent.Hero);
                            return (
                                State: s.State with { Heroes = s.State.Heroes.Replace(herointent.Hero, h) },
                                Events: s.Events.AddRange(events));
                        }
                    );

                    aborted = until(nextTuple.State, nextTuple.Events) ? true : aborted;

                    return nextTuple;
                }
            );

            return finalTuple.State;
        }

        public static (GameState, Events) DoTurn(this GameState current, IEnumerable<Func<GameState, (GameState, Events)>> instructions)
        {
            return instructions.Aggregate(
                (State: current, Events: ImmutableList<Event>.Empty),
                (last, update) => update(last.State)
            );
        }

        public static Hero AddIntent(this Hero hero, HeroIntent intent) =>
            hero with { Intents = hero.Intents.Add(intent) };

        public static Hero AddMoveIntent(this Hero hero, [NotNull] Location destination) =>
            hero with { Intents = hero.Intents.Add(MoveIntent.Create(hero, destination)) };

        public static IEnumerable<HerosIntent> ActiveIntents(this GameState state) =>
            state.Heroes
                .Select(hero => hero.Intents.Select(intent => new HerosIntent(hero, intent)))
                .SelectMany(r => r);

        // vector stuff
        public static Vector2 Subtract(this Vector2 vector, Vector2 other) => Vector2.Subtract(vector, other);
        public static Vector2 Unit(this Vector2 vector) => Vector2.Normalize(vector);
        public static Vector2 Multiply(this Vector2 vector, float factor) => Vector2.Multiply(vector, factor);
        public static Vector2 SetMagnitude(this Vector2 vector, float magnitude) =>
            vector.Unit().Multiply(magnitude);


        public static IEnumerable<int> IterateUntil(Func<bool> endCondition)
        {
            int turn = 0;
            while (!endCondition())
            {
                yield return turn++;
            }
        }
    }
}

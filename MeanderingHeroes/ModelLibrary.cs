using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;



namespace MeanderingHeroes
{
    using HerosIntent = (Hero Hero, HeroIntent Intent);
    using GameEvents = (GameState State, Events Events);

    // need to track:
    // map -> grid -> cell (weather, terrain, travel costs (n,s,e,w))
    // monsters -> monster (location, flags (hungry, scared, wounded, aggressive))
    // heroes -> hero (location, instructions, 
    public readonly record struct GameState
    {
        public Map Map { get; init; }
        public ImmutableList<Doer> Doers { get; init; }
        public GameState(Map map)
        {
            Map = map;
            Doers = [];
        }
    }

    public static partial class ModelLibrary
    {
        public static GameState RunGame(GameState initialState, Func<GameState, Events, bool> until)
        {
            bool aborted = false;

            var stop = () => aborted;

            var finalTuple = IterateUntil(stop).Aggregate(
                (State: initialState, Events: Events.Empty),
                (stateevents, _) =>
                {
                    var nextTuple = stateevents.State.ActiveIntents()
                    .Aggregate(
                        (State: stateevents.State, Events: stateevents.Events),
                        (s, herointent) =>
                        {
                            var (h, events) = herointent.Intent.HeroComputation(s.State, herointent.Hero);

                            // remove any finished intents
                            h = h with
                            {
                                Intents = h.Intents.RemoveRange(
                                    (IEnumerable<HeroIntent>)events.OfType<EndEvent>().Select(ev => ev.Intent)
                                )
                            };

                            return (
                                State: s.State with { Doers = s.State.Doers.Replace(herointent.Hero, h) },
                                Events: s.Events.AddRange(events)
                            );
                        }
                    );

                    aborted = until(nextTuple.State, (Events)nextTuple.Events) ? true : aborted;

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

        public static GameState Add(this GameState state, Doer doer)
        {
            return state with { Doers = state.Doers.Add(doer) };
        }

        public static Hero AddIntent(this Hero hero, HeroIntent intent) =>
            hero with { Intents = hero.Intents.Add(intent) };

        public static Hero AddMoveIntent(this Hero hero, NextWaypoint pathFinder) =>
            hero with { Intents = hero.Intents.Add(MoveIntent.Create(hero, pathFinder)) };

        public static IEnumerable<HerosIntent> ActiveIntents(this GameState state) =>
            state.Doers.OfType<Hero>()
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

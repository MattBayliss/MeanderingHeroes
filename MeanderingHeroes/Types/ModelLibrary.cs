using LaYumba.Functional;
using MeanderingHeroes.Types.Commands;
using MeanderingHeroes.Types.Doers;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;



namespace MeanderingHeroes
{
    using DoersIntent = (Doer Doer, Command Intent);
    using GameEvents = (GameState State, Events Events);

    // need to track:
    // map -> grid -> cell (weather, terrain, travel costs (n,s,e,w))
    // monsters -> monster (location, flags (hungry, scared, wounded, aggressive))
    // heroes -> hero (location, instructions, 
    public readonly record struct GameState
    {
        public Map Map { get; init; }
        public ImmutableList<Doer> Doers { get; init; }
        public ImmutableList<Command> Commands { get; init; }
        public GameState(Map map)
        {
            Map = map;
            Doers = [];
            Commands = [];
        }
    }

    public static partial class ModelLibrary
    {
        public static GameState RunGame(GameState initialState, Func<GameState, Events, bool> until)
        {
            bool aborted = false;

            var stop = () => aborted;

            var initialTuple = (State: initialState, Events: Events.Empty);

            var finalTuple = initialTuple.IterateUntil(
                endCondition: stateTuple => until(stateTuple.State, stateTuple.Events),
                updateFunc: stateevents =>
                {
                    var nextTuple = stateevents.State.Commands
                        .Aggregate(
                            seed: (State: stateevents.State, Events: stateevents.Events),
                            func: (gameEvents, command) =>
                                {
                                    var (newstate, events) = command.ProcessIntent(gameEvents.State);

                                    var commandsCompleted = events
                                        .OfType<EndEvent>()
                                        .Select(ee => ee.Intent);

                                    // remove any finished intents
                                    var prunedstate = newstate with
                                    {
                                        Commands = newstate.Commands.RemoveRange(commandsCompleted)
                                    };
                                

                                    return (
                                        State: prunedstate,
                                        Events: gameEvents.Events.AddRange(events)
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

        public static T AddFleeReaction<T>(this T @this, float threshold) where T : Doer
        {
            return @this with
            {
                Reactions = @this.Reactions.Add(
                    new Flee
                    {
                        Triggers = [
                            new ProximityTrigger { Threshold = threshold },
                            new ThreatTrigger { Threshold = threshold }
                        ],
                        Action = source => ([], [])
                    }
                )
            };
        }

        public static Option<T> GetDoer<T>(this GameState state, int id) where T : Doer
        {
            return state.Doers.OfType<T>().Find(d => d.Id == id);
        }

        // vector stuff
        public static Vector2 Subtract(this Vector2 vector, Vector2 other) => Vector2.Subtract(vector, other);
        public static Vector2 Unit(this Vector2 vector) => Vector2.Normalize(vector);
        public static Vector2 Multiply(this Vector2 vector, float factor) => Vector2.Multiply(vector, factor);
        public static Vector2 SetMagnitude(this Vector2 vector, float magnitude) =>
            vector.Unit().Multiply(magnitude);


        public static TState IterateUntil<TState>(
            this TState state, 
            Func<TState, bool> endCondition, 
            Func<TState, TState> updateFunc)
        {
            while (!endCondition(state))
            {
                state = updateFunc(state);
            }
            return state;
        }
    }
}

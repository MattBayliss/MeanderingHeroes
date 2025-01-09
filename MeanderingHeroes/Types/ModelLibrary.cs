using LaYumba.Functional;
using MeanderingHeroes.Types;
using MeanderingHeroes.Types.Commands;
using MeanderingHeroes.Types.Doers;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;



namespace MeanderingHeroes
{
    using DoersIntent = (Doer Doer, Command Intent);
    using GameEvents = (GameState State, Events Events);

    public static partial class ModelLibrary
    {

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

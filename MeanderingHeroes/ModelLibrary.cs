using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MeanderingHeroes;


namespace MeanderingHeroes
{
    using IntentResult = (Activity Activity, IEnumerable<Event> events);
    using HerosIntent = (Hero Hero, HeroIntent Intent);

    public static class ModelLibrary
    {
        public static float Speed(this Hero hero, GameState state)
        {
            return 4.0f;
        }

        public static IEnumerable<HerosIntent> ActiveIntents(this GameState state) =>
            state.Heroes
                .Select(hero => hero.Intents.Select(intent => new HerosIntent(hero, intent)))
                .SelectMany(r => r);


        public static IntentResult ApplyIntent(this GameState gameState, Hero hero, MeanderingHeroes.HeroIntent intent)
        {
            return (new Activity(), Enumerable.Empty<Event>());
        }

        // vector stuff
        public static Vector2 Subtract(this Vector2 vector, Vector2 other) => Vector2.Subtract(vector, other);
        public static Vector2 Unit(this Vector2 vector) => Vector2.Normalize(vector);
        public static Vector2 Multiply(this Vector2 vector, float factor) => Vector2.Multiply(vector, factor);
        public static Vector2 SetMagnitude(this Vector2 vector, float magnitude) =>
            vector.Unit().Multiply(magnitude);
    }
}

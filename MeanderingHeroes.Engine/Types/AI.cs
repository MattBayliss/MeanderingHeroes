using MeanderingHeroes.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaYumba.Functional;

namespace MeanderingHeroes.Engine.Types.AI
{
    public interface IInteraction
    {
        Utility CalculateUtility();
    }
    public abstract record Behaviour (string Name, InteractionBase Interaction)
    {
        public bool ToRemove { get; protected set; }
        public abstract SmartEntity Update(Game game, SmartEntity entity);
    }
    public delegate SmartEntity UpdateEntity(Game game, SmartEntity entity);
    public delegate Option<float> ConsiderationD(Game game, Entity entity);
    public delegate Utility Curve(float consideration);
    public delegate Utility Aggregator(IEnumerable<Utility> Utilities);
    public abstract record InteractionBase
    {
        public abstract Utility CalculateUtility(Game game, Entity entity);
    }
    public record Interaction(ConsiderationD consideration, Curve curve) : InteractionBase
    {
        public override Utility CalculateUtility(Game game, Entity entity)
        {
            return consideration(game, entity)
                 .Map(cv => curve(cv))
                 .Match(() => (Utility)0f, utility => utility);
        }
    }
    public record CombinedInteraction(IEnumerable<Interaction> Interactions, Aggregator AggregatorFunc) : InteractionBase
    {
        public override Utility CalculateUtility(Game game, Entity entity) => AggregatorFunc(Interactions.Select(i => i.CalculateUtility(game, entity)));
    }

    public class Game2
    {
        public void Update()
        {

        }
    }

    public static class Thinking
    {
        public static void Thought1()
        {
            // game loop - 
            var game = new Game(new Grid([]), new Transforms(new System.Numerics.Vector2(0f, 0f), 1, 1));
            var utilityAi = new UtilityAIComponent();
            Func<Game, UtilityAIComponent, Game> gameLoop = (game, ai) =>
            {
                var advertisers = game.Entities.OfType<Advertiser>();
                var heros = game.Entities.OfType<SmartEntity>();

                //foreach nearby advertiser - calculate utility
                // foreach player assigned Interaction/Goal - calculate utility

                return game;
            };
        }
    }

}

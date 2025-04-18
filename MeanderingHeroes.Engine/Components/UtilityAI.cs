using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using MeanderingHeroes.Engine.Types.AI;
using System.Linq;

namespace MeanderingHeroes.Engine.Components
{
    public delegate Utility EvalAggregrator(Evaluation eval1, Evaluation eval2);
    public delegate Utility EvalCurve(float consideration);
    public record Evaluation(ConsiderationD consideration, EvalCurve curve);

    public record Offer(ConsiderationD Consideration, EvalCurve Curve);
    public delegate Utility UtilityDelegate(Game game, SmartEntity entity);
    public delegate SmartEntity UpdateDelegate(Game game, SmartEntity entity);
    // c11n shorthand for consideration
    public delegate (T C11nState, SmartEntity Entity) StatefulUpdateDelegate<T>(Game game, T c11nState, SmartEntity entity);

    public record ConsiderationRecord(UtilityDelegate CalculateUtility, UpdateDelegate Update);

    public record StatefulBehaviour<T> : Behaviour
    {
        public T C11nState { get; init; }
        protected Func<SmartEntity, bool> _toRemove { get; init; }
        protected StatefulUpdateDelegate<T> _updateFunc { get; init; }
        protected InteractionBase _interaction;

        public StatefulBehaviour(string name, T c11nState, InteractionBase interaction, StatefulUpdateDelegate<T> updateFunc, Func<SmartEntity, bool> toRemove) 
            : base(name, interaction)
        {
            C11nState = c11nState;
            _updateFunc = updateFunc;
            _toRemove = toRemove;
            _interaction = interaction;
        }
        public Utility CalculateUtility(Game game, SmartEntity entity) => _interaction.CalculateUtility(game, entity);

        public override SmartEntity Update(Game game, SmartEntity entity)
        {
            var (state2, entity2) = _updateFunc(game, C11nState, entity);
            ToRemove = _toRemove(entity2);
            return entity2 with { Behaviours = entity.Behaviours.Replace(this, this with { C11nState = state2 }) };
        }
    }

    public class UtilityAIComponent : IComponent
    {
        public GameState Update2(Game game, GameState state)
        {
            var updatedEntities = state.Entities.Select(entity => entity switch
            {
                SmartEntity smartEntity => Update(game, smartEntity).Match(() => smartEntity, updatedEntity => updatedEntity),
                _ => entity
            });

            // TODO: test to see if this creates a copy of gamestate if nothing has changed
            return state with { Entities = updatedEntities.ToImmutableList() };
        }
        private Option<SmartEntity> Update(Game game, SmartEntity entity)
        {
            // TODO: filter advertisers to only those nearby entity?
            var offers = game
                .Entities
                .OfType<Advertiser>()
                .SelectMany(ad => ad.Offers)
                .Select(off => off.Consideration);



            // you should take the top 3 and randomly choose from those - depending
            // on the deviation of results

            // BUT this early on, just return the highest result every time
            var calculatedUtilities = entity.Behaviours
                .Select(b =>
                    (
                        Utility: b.Interaction.CalculateUtility(game, entity),
                        Behaviour: b
                    )
                ).OrderByDescending(cc => cc.Utility);

            var winner = calculatedUtilities.Head();

            return winner.Match(
                None: () => entity,
                Some: bhr => bhr.Behaviour.Update(game, entity)
            ).Pipe(updated => updated with 
                { Behaviours = updated.Behaviours.Where(i9n => !i9n.ToRemove).ToImmutableList() }
            );
        }
    }
}

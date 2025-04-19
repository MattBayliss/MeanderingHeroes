using MeanderingHeroes.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaYumba.Functional;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record Behaviour (string Name, InteractionBase Interaction)
    {
        public bool ToRemove { get; protected set; }
        public abstract SmartEntity Update(Game game, SmartEntity entity);
    }
    public delegate Utility EvalCurve(float consideration);

    public delegate Utility UtilityDelegate(Game game, SmartEntity entity);
    // c11n shorthand for consideration
    public delegate (T C11nState, SmartEntity Entity) StatefulUpdateDelegate<T>(Game game, T c11nState, SmartEntity entity);

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

    public delegate Option<float> ConsiderationD(Game game, Entity entity);

    public delegate SmartEntity UpdateEntity(Game game, SmartEntity entity);
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

}

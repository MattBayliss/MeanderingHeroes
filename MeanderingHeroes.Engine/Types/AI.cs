using MeanderingHeroes.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record Behaviour (string Name, InteractionBase Interaction)
    {
        public bool ToRemove { get; protected set; }
        public abstract SmartEntity Update(Game game, SmartEntity agent);
    }
    public delegate Utility EvalCurve(float consideration);

    public delegate Utility UtilityDelegate(Game game, SmartEntity agent);
    // c11n shorthand for consideration
    public delegate (T C11nState, SmartEntity Entity) StatefulUpdateDelegate<T>(Game game, T c11nState, SmartEntity agent);

    public record StatefulBehaviour<T> : Behaviour
    {
        public Option<T> State { get; init; }
        private Func<Entity, T> _initState { get; init; }
        protected Func<SmartEntity, bool> _toRemove { get; init; }
        protected StatefulUpdateDelegate<T> _updateFunc { get; init; }
        protected InteractionBase _interaction;

        public StatefulBehaviour(string name, Func<Entity, T> initState, InteractionBase interaction, StatefulUpdateDelegate<T> updateFunc, Func<SmartEntity, bool> toRemove)
            : base(name, interaction)
        {
            State = None;
            _initState = initState;
            _updateFunc = updateFunc;
            _toRemove = toRemove;
            _interaction = interaction;
        }
        public Utility CalculateUtility(Game game, SmartEntity agent, Entity target) => _interaction.CalculateUtility(game, agent, target);

        public override SmartEntity Update(Game game, SmartEntity agent)
        {
            var (state2, entity2) = _updateFunc(game, State.Match(() => _initState(agent), state => state), agent);
            ToRemove = _toRemove(entity2);
            return entity2 with { Behaviours = agent.Behaviours.Replace(this, this with { State = Some(state2) }) };
        }
    }

    public delegate Option<float> ConsiderationD(Game game, SmartEntity agent, Entity target);

    public delegate SmartEntity UpdateEntity(Game game, SmartEntity entity);
    public delegate Utility Curve(float consideration);
    public delegate Utility Aggregator(params IEnumerable<Utility> Utilities);
    public abstract record InteractionBase
    {
        public abstract Utility CalculateUtility(Game game, SmartEntity agent, Entity target);
    }
    public record Interaction(ConsiderationD consideration, Curve curve) : InteractionBase
    {
        public override Utility CalculateUtility(Game game, SmartEntity agent, Entity target)
        {
            return consideration(game, agent, target)
                 .Map(cv => curve(cv))
                 .Match(() => (Utility)0f, utility => utility);
        }
    }
    public record CombinedInteraction(IEnumerable<Interaction> Interactions, Aggregator AggregatorFunc) : InteractionBase
    {
        public override Utility CalculateUtility(Game game, SmartEntity agent, Entity target) => AggregatorFunc(Interactions.Select(i => i.CalculateUtility(game, agent, target)));
    }

}

using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;

namespace MeanderingHeroes.Engine.Components
{
    [Flags]
    public enum AdvertFlag
    {
        All,
        Mobile,
        BaseMotives,
        HigherMotives,
        Wealth,
        Greed,
        Hero = Mobile | BaseMotives | HigherMotives | Wealth
    }

    public record Offer(AdvertFlag Types, IConsideration Consideration);
    public delegate Utility UtilityDelegate(Game game, SmartEntity entity);
    public delegate SmartEntity UpdateDelegate(Game game, SmartEntity entity);
    // c11n shorthand for consideration
    public delegate (T C11nState, SmartEntity Entity) StatefulUpdateDelegate<T>(Game game, T c11nState, SmartEntity entity);
    public interface IConsideration
    {
        // TODO: Separate CalculateUtility away from the Update?
        /// <summary>
        /// Calculates a utility value (from 0.0 to 1.0) 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Utility CalculateUtility(Game grid, SmartEntity entity);
        SmartEntity Update(Game game, SmartEntity entity);
        bool ToRemove(SmartEntity entity);
    }

    public record ConsiderationRecord(UtilityDelegate CalculateUtility, UpdateDelegate Update);

    public record StatefulConsideration<T> : IConsideration
    {
        public T C11nState { get; init; }
        protected Func<SmartEntity, bool> _toRemove { get; init; }
        protected UtilityDelegate _utilityFunc { get; init; }
        protected StatefulUpdateDelegate<T> _updateFunc { get; init; }
        public StatefulConsideration(T c11nState, UtilityDelegate utilityFunc, StatefulUpdateDelegate<T> updateFunc, Func<SmartEntity, bool> toRemove)
        {
            C11nState = c11nState;
            _utilityFunc = utilityFunc;
            _updateFunc = updateFunc;
            _toRemove = toRemove;
        }
        public bool ToRemove(SmartEntity entity) => _toRemove(entity);
        public Utility CalculateUtility(Game game, SmartEntity entity) => _utilityFunc(game, entity);

        public SmartEntity Update(Game game, SmartEntity entity)
        {
            var (state2, entity2) = _updateFunc(game, C11nState, entity);
            return entity2 with { Considerations = entity.Considerations.Replace(this, this with { C11nState = state2 }) };
        }
    }

    public class UtilityAIComponent
    {
        public SmartEntity Update(Game game, SmartEntity entity)
        {
            // you should take the top 3 and randomly choose from those - depending
            // on the deviation of results

            // BUT this early on, just return the highest result every time
            var calculatedUtilities = entity.Considerations
                .Select(c =>
                    (
                        Utility: c.CalculateUtility(game, entity),
                        C11n: c
                    )
                ).OrderByDescending(cc => cc.Utility);

            var winner = calculatedUtilities.Head();

            return winner.Match(
                None: () => entity,
                Some: cns => cns.C11n.Update(game, entity)
            ).Pipe(updated => updated with 
                { Considerations = updated.Considerations.Where(c11n => !c11n.ToRemove(updated)).ToImmutableList() }
            );
        }
    }
}

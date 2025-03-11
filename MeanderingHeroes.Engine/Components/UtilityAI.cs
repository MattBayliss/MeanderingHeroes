using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Components
{
    // c11n shorthand for consideration
    public delegate float UtilityDelegate<T>(Grid grid, GameState state, T c11nState, Entity entity);
    public delegate (T C11nState, Entity Entity) UpdateDelegate<T>(T c11nState, Entity entity);
    public interface IConsideration
    {
        float CalculateUtility(Grid grid, GameState state, Entity entity);
        Entity Update(Entity entity);
        bool ToRemove(Entity entity);
    }
    public record StatefulConsideration<T> : IConsideration
    {
        public T C11nState { get; init; }
        protected Func<Entity, bool> _toRemove { get; init; }
        protected UtilityDelegate<T> _utilityFunc { get; init; }
        protected UpdateDelegate<T> _updateFunc { get; init; }
        public StatefulConsideration(T c11nState, UtilityDelegate<T> utilityFunc, UpdateDelegate<T> updateFunc, Func<Entity, bool> toRemove)
        {
            C11nState = c11nState;
            _utilityFunc = utilityFunc;
            _updateFunc = updateFunc;
            _toRemove = toRemove;
        }
        public bool ToRemove(Entity entity) => _toRemove(entity);
        public float CalculateUtility(Grid grid, GameState state, Entity entity) => _utilityFunc(grid, state, C11nState, entity);

        public Entity Update(Entity entity)
        {
            var (state2, entity2) = _updateFunc(C11nState, entity);
            return entity2 with { Considerations = entity.Considerations.Replace(this, this with { C11nState = state2 }) };
        }
    }

    public class UtilityAIComponent(Grid Grid)
    {

        public Entity Update(GameState gameState, Entity entity)
        {
            // you should take the top 3 and randomly choose from those - depending
            // on the deviation of results

            // BUT this early on, just return the highest result every time
            var calculatedUtilities = entity.Considerations
                .Select(c =>
                    (
                        Utility: c.CalculateUtility(Grid, gameState, entity),
                        C11n: c
                    )
                ).OrderByDescending(cc => cc.Utility);

            var winner = calculatedUtilities.Head();

            return winner.Match(
                None: () => entity,
                Some: cns => cns.C11n.Update(entity)
            ).Pipe(updated => updated with 
                { Considerations = updated.Considerations.Where(c11n => !c11n.ToRemove(updated)).ToImmutableList() }
            );
        }
    }
}

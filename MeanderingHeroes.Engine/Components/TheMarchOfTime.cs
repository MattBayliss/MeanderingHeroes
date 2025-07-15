using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Components
{
    internal static class TheMarchOfTime
    {
        private const float DRAIN_AMOUNT = 0.01f;
        public static GameState Update(GameState state)
            => state.UpdateState(
                state.Entities
                    .Select(entity => entity switch
                    {
                        { Hunger.Value: 1f, Constitution: var con } when con <= DRAIN_AMOUNT => entity, // TODO: implement death
                        { Hunger.Value: 1f, Constitution: var con } => entity with { Constitution = con - DRAIN_AMOUNT },
                        { Hunger: var hunger } => entity with { Hunger = hunger + DRAIN_AMOUNT }
                    })
                    .Select(entity => new EntityChange(entity)), []
                );
    }
}

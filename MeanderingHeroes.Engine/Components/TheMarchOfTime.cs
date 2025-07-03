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
        public static GameState Update(GameState state) => state.ModifyEntities(state.Entities
                .Select(entity => entity switch
                {
                    { Hunger.Value: 1f, Constitution: var con } when con <= 0.001f => entity, // TODO: implement death
                    { Hunger.Value: 1f, Constitution: var con } => entity with { Constitution = con - 0.001f },
                    { Hunger: var hunger } => entity with { Hunger = hunger + 0.001f }
                }));
    }
}

using LaYumba.Functional;
using MeanderingHeroes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Components
{
    public record Consideration(
        // to give considerations "stickiness"
        long RunningTicks,
        Func<Grid, GameState, long, Entity, float> CalculateUtility, 
        Func<Entity, Entity> UpdateEntity
    );


    public class UtilityAIComponent(Grid Grid)
    {
        public Entity Update(GameState gameState, Entity entity)
        {
            // you should take the top 3 and randomly choose from those - depending
            // on the deviation of results

            // BUT this early on, just return the highest result every time
            var winner = entity.Considerations
                .Select(c =>
                    (
                        Utility: c.CalculateUtility(Grid, gameState, c.RunningTicks, entity),
                        UpdateFunc: c.UpdateEntity
                    )
                ).OrderByDescending(cc => cc.Utility)
                .Head(); // returns an Option<(float, Func<Entity, Entity>)

            return winner.Match(
                None: () => entity,
                Some: cns => cns.UpdateFunc(entity)
            );
        }
    }
}

using MeanderingHeroes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Components
{
    // using terminology from https://youtu.be/H6QEpc2SQiY - and plan to implement some
    // of the fuzzy logic ideas presented in that presentation
    public record Consideration;
    public record Curve;
    public record Evaluator(Consideration Considerations, Curve Curve);
    public record Aggregator(ImmutableList<Evaluator> Evalutators, Func<ImmutableList<Evaluator>> AggregateFunc);

    public class UtilityAIComponent
    {
        public void Update(Entity entity)
        {
            
        }
    }
}

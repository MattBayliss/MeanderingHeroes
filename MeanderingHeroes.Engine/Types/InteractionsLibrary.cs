using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public static class InteractionsLibrary
    {
        public static Interaction Nearby = new Interaction(ConsideratonsLibrary.Nearby, CurvesLibrary.Distance);
        public static Interaction Desirability(float valueInGold) => new Interaction(ConsideratonsLibrary.AcquireWealth, CurvesLibrary.Linear(0f, 1f, valueInGold));
        public static CombinedInteraction DesirabilityOverDistance(float valueInGold) => new CombinedInteraction([Nearby, Desirability(valueInGold)], AggregatorsLibrary.Average);
    }
}

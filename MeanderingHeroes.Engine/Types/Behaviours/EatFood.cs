using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types.Behaviours
{
    public static partial class BehavioursLibrary
    {
        public static BehaviourTemplate EatFood => game => pawn => new Behaviour(EatFoodDse, EatFoodFromSupplyCommand);
        private static Dse EatFoodDse => new("EatFood", "Eat food from current supplies", 2f, [
                new Decision(ConsiderationType.Hunger, CurveLibrary.NotUrgentUntilItIs),
                new Decision(ConsiderationType.FoodSupply, CurveLibrary.SupplyLogistic)
            ]);

        private static Command EatFoodFromSupplyCommand => (pawn, _)
            => new AiResult(
                EntityChange: Some(pawn with { FoodSupply = pawn.FoodSupply - 0.1f, Hunger = pawn.Hunger - 0.1f }),
                Status: DseStatus.Running
                );

    }
}

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
        public static BehaviourTemplate GatherFood
            => game => pawn => new(
                GatherFoodDse,
                GatherFoodAtLocation(game)
        );

        private static Dse GatherFoodDse => new(
                name: "GatherFood",
                description: "forage for nearby food",
                weight: 2f,
                decisions: 
                    [
                        new Decision(ConsiderationType.FoodSupply, CurveLibrary.NegativeLinear),
                        // Needs to be 1 when we're close enough (within 0.05) and 0 otherwise
                        new Decision(ConsiderationType.ForageFoodDistance, CurveLibrary.WithinInteractionRange)
                    ]
                );

        private static Command GatherFoodAtLocation(Game game) =>
            (entity, state) => game.Blackboard.Get(BlackboardKeys.ClosestForageFood(entity.HexCoords.Round()))
                .Match(
                    None: () => new AiResult(None, DseStatus.Aborted),
                    // TODO: actually need to decrease the food supply in that layer
                    Some: ff => new AiResult(
                            EntityChange: Some(entity with { FoodSupply = entity.FoodSupply + 0.5f }),
                            Status: DseStatus.Running
                        )
                    );
    }
}

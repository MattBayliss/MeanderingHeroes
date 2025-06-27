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
        public static BehaviourTemplate MoveToForageFood
            => game => pawn => new(
                MoveToForageFoodDse,
                MoveToForageFoodUpdateFunc(game)
        );

        private static Dse MoveToForageFoodDse => new(
                name: "MoveToForageFood",
                description: "move to closest food to forage",
                weight: 2f,
                decisions: 
                    [
                        new Decision(ConsiderationType.Hunger, CurveLibrary.BasicLinear),
                        new Decision(ConsiderationType.FoodSupply, CurveLibrary.ReverseLogistic),

                        // TODO: Needs to be 0 when we're close enough so a "Forage" DSE can take over
                        new Decision(ConsiderationType.ForageFoodDistance, new(CurveType.Step, 0f, 1f, 0.05f, 0f)),
                        new Decision(ConsiderationType.ForageFoodDistance, CurveLibrary.LogisticTrailOff)
                    ]
                );

        private static Command MoveToForageFoodUpdateFunc(Game game) =>
            (entity, state) => game.Blackboard.Get(BlackboardKeys.ClosestForageFood(entity.HexCoords.Round()))
                .Match(
                    None: () => new AiResult(None, DseStatus.Aborted),
                    Some: foodCoords => PathFinding.GeneratePathGoalBehaviour(game, entity.HexCoords, foodCoords, _ => DseStatus.Running)(entity, state)
                );
    }
}

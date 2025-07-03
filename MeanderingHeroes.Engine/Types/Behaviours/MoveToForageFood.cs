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

                        // TODO: Needs to be 0 when we're at food so that "Forage" DSE can take over
                        new Decision(ConsiderationType.ForageFoodDistance, CurveLibrary.IsNotZero),
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

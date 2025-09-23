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
                        // TODO: check to make sure ForageFoodDistance takes into account AwareOfFood
                        new Decision(ConsiderationType.Hunger, CurveLibrary.BasicLinear),
                        new Decision(ConsiderationType.FoodSupply, CurveLibrary.ReverseLogistic),
                        new Decision(ConsiderationType.ForageFoodDistance, CurveLibrary.IsNotZero),
                        new Decision(ConsiderationType.ForageFoodDistance, CurveLibrary.LogisticTrailOff)
                    ]
                );

        private static Command MoveToForageFoodUpdateFunc(Game game) =>
            (entity, state) => game.Blackboard.Get(BlackboardKeys.ClosestForageFood(entity))
                .Match(
                    None: () => new AiResult([], DseStatus.Aborted),
                    Some: foodHex => PathFinding.GeneratePathGoalBehaviour(game, entity.HexCoords, foodHex, _ => DseStatus.Running)(entity, state)
                );
    }
}

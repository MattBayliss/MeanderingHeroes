using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types.Behaviours
{
    public static partial class BehavioursLibrary
    {
        /// <summary>
        /// Search for food needs to determine neighbour hex terrain types, and travel to one
        /// most likely to have food, and that hasn't been searched recently.
        /// Searching also needs to take a number of turns, and not be abandoned - i.e. some
        /// good inertia to it
        /// </summary>
        public static BehaviourTemplate TravelToSearchForFood
            => game => pawn => new(
                MoveToForageFoodDse,
                MoveToForageFoodUpdateFunc(game)
        );

        private static Dse TravelToSearchForFoodDse => new(
                name: "TravelToSearchForFood",
                description: "move to closest unsearched hex",
                weight: 2f,
                decisions:
                    [
                        // TODO: check to make sure ForageFoodDistance takes into account AwareOfFood
                        new Decision(ConsiderationType.Hunger, CurveLibrary.BasicLinear),
                        new Decision(ConsiderationType.FoodSupply, CurveLibrary.ReverseLogistic),
                        new Decision(ConsiderationType.ClosestPotentialFoodHex, CurveLibrary.IsNotZero),
                        new Decision(ConsiderationType.ForageFoodDistance, CurveLibrary.LogisticTrailOff)
                    ]
                );

        private static Command TravelToSearchForFoodUpdateFunc(Game game) =>
            (entity, state) => game.Blackboard.Get(BlackboardKeys.ClosestForageFood(entity))
                .Match(
                    None: () => new AiResult([], DseStatus.Aborted),
                    Some: foodHex => PathFinding.GeneratePathGoalBehaviour(game, entity.HexCoords, foodHex, _ => DseStatus.Running)(entity, state)
                );


    }
}

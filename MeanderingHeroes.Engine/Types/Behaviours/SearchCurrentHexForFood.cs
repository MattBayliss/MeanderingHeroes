using LaYumba.Functional;
using MeanderingHeroes.Engine.Types.Skills;

namespace MeanderingHeroes.Engine.Types.Behaviours
{
    public static partial class BehavioursLibrary
    {
        /// <summary>
        /// Searching a hex for food should take a number of turns.
        /// </summary>
        public static BehaviourTemplate SearchCurrentHexForFood => game => pawn =>
        {
            // TODO: ticks to try looking for food should be a calculation of tenacity and/or desperation
            var giveUpAtTick = Game.Tick + 10;
            var hexCoords = pawn.HexCoords;
            return new
            (
                Dse: SearchCurrentHexForFoodDse(pawn.HexCoords),
                Command: SearchForFoodCommand(hexCoords.Round(), game.KnowledgeBase, giveUpAtTick)
            );
        };

        private static Command SearchForFoodCommand(Hex hex, KnowledgeBase knowledgeBase, long giveUpAtTick) => (entity, state) =>
        {
            var foodFound = state
                .FoodItems
                .ItemsByHex
                .Lookup(hex)
                .Bind
                    ( foodItems => 
                        foodItems
                            .Where(fi => PlayerSkills.ChanceOfFindingFood((FoodType)fi.SubType)(entity).Value > Game.Random2dDistribution().Value)
                            .Head()
                    );

            return foodFound.Match(
                // nothing found, give up if time limit was reached
                None: () => Game.Tick >= giveUpAtTick ?
                    // given up
                    new AiResult
                    (
                        StateChanges: [StateChange.TidbitLearnt(entity.Id, ConsiderationType.HexFood, hex, 0f)],
                        Status: DseStatus.Completed
                    ) :
                    // still going
                    new AiResult
                    (
                        StateChanges: [],
                        Status: DseStatus.Running
                    ),

                // food has been found
                Some: food => new AiResult
                (
                    StateChanges: [
                        // mark knowledge of layer item
                        StateChange.LayerItemFound(entity.Id, food),
                        // mark hex as having been searched for food
                        StateChange.TidbitLearnt(entity.Id, ConsiderationType.HexFood, hex, food.Quality)],
                    Status: DseStatus.Completed)
                );
        };

        private static Dse SearchCurrentHexForFoodDse(FractionalHex hexCoords) => new(
                name: "SearchHexForFood",
                description: "search immediate area for food to eat",
                weight: 2f,
                decisions:
                    [
                        new Decision(ConsiderationType.Hunger, CurveLibrary.BasicLinear),
                        new Decision(ConsiderationType.FoodSupply, CurveLibrary.ReverseLogistic),
                        // If entity doesn't know there's food here (HexFood is zero), keep searching
                        new DecisionOnHex(ConsiderationType.HexFood, CurveLibrary.IsZero, hexCoords),
                        // while there's a chance there's undiscovered food here, keep searching
                        new DecisionOnHex(ConsiderationType.PotentialFoodAtHex, CurveLibrary.IsNotZero, hexCoords)
                    ]
                );
    }
}

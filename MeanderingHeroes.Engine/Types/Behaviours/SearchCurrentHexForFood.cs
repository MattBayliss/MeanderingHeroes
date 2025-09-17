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
            var awareOfFood = state
                .FoodItems.ItemsByHex[hex]
                .Any(fi => PlayerSkills.ChanceOfFindingFood((FoodType)fi.SubType)(entity).Value > Game.Random2dDistribution().Value);

            var attemptResult = (awareOfFood || Game.Tick >= giveUpAtTick) ? Some(awareOfFood) : None;

            return attemptResult.Match(
                // nothing found, and haven't given up yet
                None: () => new([], DseStatus.Running), 

                // either entity found food, or has given up
                Some: aware => new AiResult([StateChange.TidbitLearnt(ConsiderationType.HexFood, hex, awareOfFood ? 1f : 0f)], DseStatus.Completed));
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

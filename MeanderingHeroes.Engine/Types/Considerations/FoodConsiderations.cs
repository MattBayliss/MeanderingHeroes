using LaYumba.Functional;
using static LaYumba.Functional.F;
using static MeanderingHeroes.Engine.Types.Skills.PlayerSkills;
using static MeanderingHeroes.Engine.Functions;

namespace MeanderingHeroes.Engine.Types
{
    public static partial class BlackboardKeys
    {
        public static BlackboardKey<FractionalHex> ClosestForageFood(Entity entity) => new($"ClosestForageFood.{entity.Id}", false);
        /// <summary>
        /// AwareOfFood has three states: 
        ///     Some(true)  - entity knows food exists at hex; 
        ///     Some(false) - entity couldn't find any food at hex;
        ///     None        - entity hasn't finished searching hex for food;
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static BlackboardKey<Option<bool>> AwareOfFood(Entity entity, Hex hex) => new($"AwareOfFood.{entity.Id}.{hex}", true);
    }
}
namespace MeanderingHeroes.Engine.Types.Considerations
{
    internal static class FoodHelpers
    {
        public static Func<Hex, Option<Hex>> BestNextHexToSearchForFood(this KnowledgeBase knowledgeBase, int entityId)
            => currentHex
                => knowledgeBase
                    .GetTidbit(entityId, ConsiderationType.ClosestPotentialFoodHex, currentHex)
                    .Map(tb => tb.HexValue);

        public static Func<LayerItem, bool> KnowsAboutFoodItem(this KnowledgeBase knowledgeBase, int entityId)
            => foodItem
                => knowledgeBase.GetTidbit(entityId, ConsiderationType.HexFood, foodItem.HexCoords.Round()).IsSome();
        public static Func<Hex, bool> HasntSearchedHex(this KnowledgeBase knowledgeBase, int entityId)
            => hex
                => knowledgeBase.GetTidbit(entityId, ConsiderationType.HexFood, hex) == None;
    }
    public class HexFood : HexConsideration
    {
        public HexFood(ConsiderationContext context, Hex hex) : base(context, hex) { }
        public override GetConsideration Get =>
            entity =>
            {
                var knowledgeOfFoodAtHex = Context.KnowledgeBase.GetTidbit(entity.Id, ConsiderationType.HexFood, Hex);

                return knowledgeOfFoodAtHex.Map(aware => aware.UtilityValue);
            };
    }
    public class PotentialFoodAtHex : HexConsideration
    {
        public PotentialFoodAtHex(ConsiderationContext context, Hex hex) : base(context, hex) { }
        public override GetConsideration Get
            => entity
                => Context.KnowledgeBase
                    .GetTidbit(entity.Id, ConsiderationType.HexFood, Hex)
                    .Match
                        (
                            None: () => Utility(1f),
                            Some: hf => hf.UtilityValue
                        );

    }

    public class ClosestPotentialFoodHex : HexConsideration
    {
        public ClosestPotentialFoodHex(ConsiderationContext context, Hex hex) : base(context, hex) { }
        public override GetConsideration Get =>
            entity => FindBestClosestFoodHexCandidate(entity.Id)
                .Bind(hex => ConsiderationContext.DistanceToHex(hex)(entity))
                //TODO: Need to save the resulting hex to the blackboard in case this consideration wins
                // and we need to go to there

                // because of the unknown factor, should get a lower score
                .Map(score => score * 0.6f);

        private Option<Hex> FindBestClosestFoodHexCandidate(int entityId)
        {
            // TODO: need to spiral out, only considering hexes the entity hasn't searched yet - different terrains
            // will get different weights, and the further out, the lower the weight also...
            return Hex.Spiral(5)
                // TODO: need access to game map to test to see if hex is in bounds
                .Where(Context.KnowledgeBase.HasntSearchedHex(entityId))
                .Head();
        }

    }

    public class ForageFoodDistance(ConsiderationContext context) : Consideration(context)
    {
        public override GetConsideration Get
            => pawn =>
            {
                var hexDistance = Context
                    .KnowledgeBase[pawn.Id]
                    .Bind(k => k.Tidbits
                        .Where(tb => tb.Consideration == ConsiderationType.HexFood)
                        .Select(tb => (Hex: tb.CoordsValue, Distance: ConsiderationContext.DefaultDistanceUtility(tb.CoordsValue, pawn.HexCoords)))
                        .OrderByDescending(hd => hd.Distance.Value)
                        .Head()
                    );
                hexDistance.ForEach(hd =>
                    Context.Blackboard.Set<FractionalHex>(BlackboardKeys.ClosestForageFood(pawn), hd.Hex));

                return hexDistance.Map(hd => hd.Distance);
            };
    }
}


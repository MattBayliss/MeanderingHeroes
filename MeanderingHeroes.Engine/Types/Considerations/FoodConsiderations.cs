using LaYumba.Functional;
using static LaYumba.Functional.F;
using static MeanderingHeroes.Engine.Types.Skills.PlayerSkills;
using static MeanderingHeroes.Engine.Functions;
using Microsoft.Extensions.Logging;

namespace MeanderingHeroes.Engine.Types
{
    public static partial class BlackboardKeys
    {
        public static BlackboardKey<LayerItemSnapshot> ClosestForageFood(Entity entity) => new($"ClosestForageFood.{entity.Id}");
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
                => knowledgeBase
                    .GetMatchingKnownLayerItems(entityId, layerItem => layerItem.Id == foodItem.Id)
                    .Any();
        public static Func<Hex, bool> HasntSearchedHex(this KnowledgeBase knowledgeBase, int entityId)
            => hex
                => knowledgeBase.GetTidbit(entityId, ConsiderationType.HexFood, hex) == None;
    }
    public class HexFood : HexConsideration
    {
        public HexFood(ConsiderationContext context, Hex hex) : base(context, hex) { }
        protected override Option<Utility> GetConsideration(Entity entity) =>
            Context.KnowledgeBase
                .GetMatchingKnownLayerItems(
                    entity.Id,
                    layerItem => layerItem.ItemType == LayerItemType.Food && layerItem.Hex == entity.Hex)
                .OrderByDescending(fi => fi.Quality)
                .Select(fi => Utility(fi.Quality))
                .Head();
    }
    public class PotentialFoodAtHex : HexConsideration
    {
        public PotentialFoodAtHex(ConsiderationContext context, Hex hex) : base(context, hex) { }
        protected override Option<Utility> GetConsideration(Entity entity)
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
        protected override Option<Utility> GetConsideration(Entity entity) 
            => FindBestClosestFoodHexCandidate(entity.Id)
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
        protected override Option<Utility> GetConsideration(Entity pawn)
        {
            var foodAndDistance = Context
                .KnowledgeBase[pawn.Id]
                .Bind(k => k.LayerItems
                    .Where(li => li.ItemType == LayerItemType.Food)
                    .Select(li => (FoodItem: li, Distance: ConsiderationContext.DefaultDistanceUtility(li.HexCoords, pawn.HexCoords)))
                    .OrderByDescending(fd => fd.Distance.Value)
                    .Head()
                );
            foodAndDistance.ForEach(fd => Context.Blackboard.Set<LayerItemSnapshot>(BlackboardKeys.ClosestForageFood(pawn), fd.FoodItem));

            return foodAndDistance.Map(hd => hd.Distance);
        }
    }
}


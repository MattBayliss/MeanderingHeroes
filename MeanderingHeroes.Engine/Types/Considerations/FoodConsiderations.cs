using LaYumba.Functional;
using static LaYumba.Functional.F;
using static MeanderingHeroes.Engine.Types.Skills.PlayerSkills;
using static MeanderingHeroes.Engine.Functions;

namespace MeanderingHeroes.Engine.Types
{
    public static partial class BlackboardKeys
    {
        public static BlackboardKey<LayerItem> ClosestForageFood(Entity entity) => new($"ClosestForageFood.{entity.Id}", false);
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
                .Map(score =>  score * 0.6f);

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
                var hex = pawn.HexCoords.Round();

                var bbKey = BlackboardKeys.ClosestForageFood(pawn);

                // looks for a value for food distance already on the blackboard, otherwise calculates
                // distance and returns a new record to add to the blackboard
                (var cDistance, var bbItem) = Context.Blackboard.Get(bbKey)
                    .Match(
                        None: () => GetClosestFoodItem(pawn).Match(
                            None: () => ((Utility)1f, Some(new LayerItem((-1000f, -1000f), LayerItemType.Food, 0, 0))),
                            Some: fi => (ConsiderationContext.DistanceToHex(fi.FoodItem.HexCoords)(pawn), Some(fi.FoodItem))
                            ),
                        Some: bbFoodItem => (ConsiderationContext.DistanceToHex(bbFoodItem.HexCoords)(pawn), None));

                // if there's an item to add to the Blackboard, do it
                bbItem.ForEach(bb => Context.Blackboard.Set(bbKey, bb));

                return cDistance;
            };

        private Option<(LayerItem FoodItem, Utility Distance)> GetClosestFoodItem(Entity pawn)
            => Context.StateSnapshot
                    .FoodItems.Items
                    .Where(Context.KnowledgeBase.KnowsAboutFoodItem(pawn.Id))
                    .Select(fi => (FoodItem: fi, Distance: ConsiderationContext.DistanceToHex(fi.HexCoords)(pawn).GetOrElse(1f)))
                    .OrderBy(fi => fi.Distance)
                    .Head();
        
    }
}


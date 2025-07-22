using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types.Considerations
{
    public class ForageFoodDistance : Consideration
    {
        public ForageFoodDistance(ConsiderationContext context) : base(context) { }
        public override GetConsideration Get
            => pawn =>
            {
                var hex = pawn.HexCoords.Round();

                var bbKey = BlackboardKeys.ClosestForageFood(hex);

                // looks for a value for food distance already on the blackboard, otherwise calculates
                // distance and returns a new record to add to the blackboard
                (var cDistance, var bbItem) = _context.Blackboard.Get(bbKey)
                    .Match(
                        None: () => GetClosestFoodItem(pawn).Match(
                            None: () => ((Utility)1f, Some(new LayerItem((-1000f, -1000f), LayerItemType.Food, 0, 0))),
                            Some: fi => (ConsiderationContext.DistanceToHex(fi.FoodItem.HexCoords)(pawn), Some(fi.FoodItem))
                            ),
                        Some: bbFoodItem => (ConsiderationContext.DistanceToHex(bbFoodItem.HexCoords)(pawn), None));

                // if there's an item to add to the Blackboard, do it
                bbItem.ForEach(bb => _context.Blackboard.Set(bbKey, bb));

                return cDistance;
            };

        private Option<(LayerItem FoodItem, Utility Distance)> GetClosestFoodItem(Entity pawn) 
            => _context.StateSnapshot
                    .LayerItems
                    .Select(fi => (FoodItem: fi, Distance: ConsiderationContext.DistanceToHex(fi.HexCoords)(pawn)))
                    .OrderByDescending(fi => fi.Distance)
                    .Head();
    }
}


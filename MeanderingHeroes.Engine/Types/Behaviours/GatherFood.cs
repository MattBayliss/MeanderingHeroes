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
        public static BehaviourTemplate GatherFood
            => game => pawn => new(
                GatherFoodDse(pawn.HexCoords),
                GatherFoodAtLocation(game)
        );

        private static Dse GatherFoodDse(FractionalHex hexCoords) => new(
                name: "GatherFood",
                description: "forage for nearby food",
                weight: 2f,
                decisions: 
                    [
                        new DecisionOnHex(ConsiderationType.HexFood, CurveLibrary.IsNotZero, hexCoords),
                        new Decision(ConsiderationType.FoodSupply, CurveLibrary.NegativeLinear)
                    ]
                );

        private static Command GatherFoodAtLocation(Game game) =>
            (entity, state) 
                => entity
                    .Pipe(e => (EntityId: e.Id, EntityHex: e.HexCoords.Round()))
                    .Pipe(eh => 
                        game.KnowledgeBase
                        .GetMatchingKnownLayerItems(eh.EntityId, li => li.ItemType == LayerItemType.Food && li.Hex == eh.EntityHex)
                        .OrderByDescending(li => li.Quality)
                        .Head())
                    .Bind(li => game.GetLayerItem(li.Id))
                    .Match(
                        None: () => new AiResult([], DseStatus.Aborted),
                        Some: ff => new AiResult(
                                StateChanges: [
                                    new EntityChange(entity with { FoodSupply = entity.FoodSupply + 0.5f }),
                                    new LayerItemChange(ff, ff with {Quality = MathF.Max(0f, ff.Quality - 0.2f) }),
                                    StateChange.LayerItemFound(entity.Id, ff)
                                ],
                                Status: DseStatus.Running
                            )
                        );

    }
}

using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public static class BehavioursLibrary
    {
        public static BehaviourTemplate PlayerSetDestination(Hex destination)
            => game => pawn => new(
                DseLibrary.PlayerSetDestinationDSE(destination),
                PathFinding.GeneratePathGoalBehaviour(game, pawn.HexCoords, destination));

        public static BehaviourTemplate MoveToForageFood()
            => game => pawn => new(
                DseLibrary.MoveToForageFood(),
                MoveToForageFoodUpdateFunc(game)
            );

        private static Command MoveToForageFoodUpdateFunc(Game game) =>
            (entity, state) => game.Blackboard.Get(BlackboardKeys.ForageFoodDistance(entity.HexCoords.Round()))
                .Match(
                    None: () => new AiResult(None, DseStatus.Aborted),
                    Some: ff => PathFinding.GeneratePathGoalBehaviour(game, entity.HexCoords, ff.HexCoords)(entity, state)
                );
    }
}

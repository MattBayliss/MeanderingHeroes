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
                (entity, state) => new(None, DseStatus.Running)
                
            );
    }
}

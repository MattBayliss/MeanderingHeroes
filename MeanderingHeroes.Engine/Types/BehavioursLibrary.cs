namespace MeanderingHeroes.Engine.Types
{
    public static class BehavioursLibrary
    {
        public static BehaviourTemplate PlayerSetDestination(Hex destination) 
            => (game, pawn) =>  new(
                DseLibrary.PlayerSetDestinationDSE(destination), 
                PathFinding.GeneratePathGoalBehaviour(game, pawn.Hex, destination));
    }
}

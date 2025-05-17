namespace MeanderingHeroes.Engine.Types
{
    public delegate Utility GetConsideration(Entity pawn);

    public class ConsiderationContext
    {
        public GetConsideration GetConsideration(Decision forDecision) => forDecision switch
        {
            { ConsiderationType: ConsiderationType.PawnSpeed } => entity => entity is Entity pawn ? Math.Clamp(pawn.Speed, 0f, 1f) : 0f,
            { ConsiderationType: ConsiderationType.PawnAvarice } => PawnAvarice,
            DecisionOnHex { ConsiderationType: ConsiderationType.HexDistance, Target: var hex } => DistanceToHex(hex),
            DecisionOnEntity { ConsiderationType: ConsiderationType.TargetDistance, Target: var target } => DistanceToTarget(target),
            _ => _ => 0f
        };

        public static GetConsideration PawnAvarice => _ => 0.5f; //Math.Clamp(entity.Greed * 1000f / entity.Wealth, 0f, 1f);
        public GetConsideration DistanceToHex(FractionalHex hex) => pawn
            => Math.Clamp(hex.Distance(pawn.HexCoords) / 10f, 0f, 1f); // anything over 10 hexes away is considered 1.0
        public GetConsideration DistanceToTarget(Entity target) => DistanceToHex(target.HexCoords);
    }

    public enum ConsiderationType
    {
        PawnAvarice,
        PawnSpeed,
        TargetDistance,
        HexDistance
    }
}

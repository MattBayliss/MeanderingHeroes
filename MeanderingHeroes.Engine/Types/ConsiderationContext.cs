namespace MeanderingHeroes.Engine.Types
{
    public delegate Utility GetConsideration(Entity pawn);

    public class ConsiderationContext
    {
        public GetConsideration GetConsideration(Decision forDecision) => forDecision switch
        {
            { ConsiderationType: ConsiderationType.PawnSpeed } => entity => entity is Entity pawn ? Math.Clamp(pawn.Speed, 0f, 1f) : 0f,
            { ConsiderationType: ConsiderationType.PawnAvarice } => PawnAvarice,
            { ConsiderationType: ConsiderationType.Hunger } => PawnHunger,
            { ConsiderationType: ConsiderationType.FoodSupply } => FoodSupply,
            DecisionOnHex { ConsiderationType: ConsiderationType.HexDistance, Target: var hex } => DistanceToHex(hex),
            DecisionOnEntity { ConsiderationType: ConsiderationType.TargetDistance, Target: var target } => DistanceToTarget(target),
            _ => _ => 0f
        };
        public static GetConsideration FoodSupply => pawn => pawn.FoodSupply / 5f;
        public static GetConsideration PawnHunger => pawn => pawn.Hunger;
        public static GetConsideration PawnAvarice => _ => 0.5f; //Math.Clamp(entity.Greed * 1000f / entity.Wealth, 0f, 1f);
        public GetConsideration DistanceToHex(FractionalHex hex) => pawn
            => Math.Clamp(hex.Distance(pawn.HexCoords) / 10f, 0f, 1f); // anything over 10 hexes away is considered 1.0
        public GetConsideration DistanceToTarget(Entity target) => DistanceToHex(target.HexCoords);
    }

    public static partial class Extensions {
        public static GetConsideration Multiply(this GetConsideration first, GetConsideration second) => entity => first(entity) * second(entity);
    }

    public enum ConsiderationType
    {
        PawnAvarice,
        PawnSpeed,
        TargetDistance,
        HexDistance,
        Hunger,
        Hex,
        FoodSupply,
        ForageFoodDistance,
        PreyAnimal
    }
}

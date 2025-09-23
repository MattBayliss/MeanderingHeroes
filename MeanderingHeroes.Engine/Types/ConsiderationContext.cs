using LaYumba.Functional;
using MeanderingHeroes.Engine.Types.Considerations;
using static LaYumba.Functional.F;
using static MeanderingHeroes.Engine.Functions;

namespace MeanderingHeroes.Engine.Types
{
    public delegate Option<Utility> GetConsideration(Entity pawn);

    public class ConsiderationContext
    {
        private readonly Game _game;
        public GameState StateSnapshot { get; private set; }
        public Blackboard Blackboard => _game.Blackboard;
        public KnowledgeBase KnowledgeBase => _game.KnowledgeBase;

        public ConsiderationContext(Game game)
        {
            _game = game;
            StateSnapshot = _game.GameState;
        }        

        public void SetStateSnapshot()
        {
            StateSnapshot = _game.GameState;
        }
        public GetConsideration GetConsideration(Decision forDecision) => forDecision switch
        {
            { ConsiderationType: ConsiderationType.PawnSpeed } => pawn => Utility(pawn.Speed),
            { ConsiderationType: ConsiderationType.PawnAvarice } => PawnAvarice,
            { ConsiderationType: ConsiderationType.Hunger } => PawnHunger,
            { ConsiderationType: ConsiderationType.FoodSupply } => FoodSupply,
            { ConsiderationType: ConsiderationType.ForageFoodDistance } => ForageFoodDistance,
            DecisionOnHex { ConsiderationType: ConsiderationType.HexFood, Target: var hex } => HexFood(hex),
            DecisionOnHex { ConsiderationType: ConsiderationType.PotentialFoodAtHex, Target: var hex } => PotentialFoodAtHex(hex),
            DecisionOnHex { ConsiderationType: ConsiderationType.HexDistance, Target: var hex } => DistanceToHex(hex),
            _ => throw new ArgumentException($"Unexpected ConsiderationType: {forDecision.ConsiderationType.ToString()}")
        };
        public static GetConsideration FoodSupply => pawn => Utility(pawn.FoodSupply / 5f);
        public static GetConsideration PawnHunger => pawn => pawn.Hunger;
        public static GetConsideration PawnAvarice => _ => Utility(0.5f); //Math.Clamp(entity.Greed * 1000f / entity.Wealth, 0f, 1f);
        public static GetConsideration DistanceToHex(FractionalHex hex) => pawn => DefaultDistanceUtility(hex, pawn.HexCoords);
        public static GetConsideration DistanceToTarget(Entity target) => pawn => DefaultDistanceUtility(target.HexCoords, pawn.HexCoords);
        public static Utility DefaultDistanceUtility(FractionalHex from, FractionalHex to)
            // anything over 10 hexes away is considered 1.0
            => from.Distance(to) / 10f;
        public Consideration HexFood(FractionalHex hex) => new HexFood(this, hex.Round());
        public Consideration ForageFoodDistance => new ForageFoodDistance(this);
        public Consideration PotentialFoodAtHex(FractionalHex hex) => new PotentialFoodAtHex(this, hex.Round());
    }
    /// <summary>
    /// A enum to easily serialize behaviours in separate data file for easy tweaking/creating of
    /// behaviours
    /// </summary>
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
        HexFood,
        PotentialFoodAtHex,
        PreyAnimal,
        ClosestPotentialFoodHex,
        ConfidenceOfFindingForageFood,
    }
    public abstract class StatelessConsideration
    {
        public abstract GetConsideration Get { get; }
        public static implicit operator GetConsideration(StatelessConsideration consideration) => consideration.Get;
    }
    public abstract class Consideration : StatelessConsideration
    {
        protected ConsiderationContext Context { get; init; }
        public Consideration(ConsiderationContext context)
        {
            Context = context;
        }
    }
    public abstract class HexConsideration : Consideration
    {
        protected Hex Hex { get; init; }
        public HexConsideration(ConsiderationContext context, Hex hex) : base(context) => Hex = hex;
    }
}

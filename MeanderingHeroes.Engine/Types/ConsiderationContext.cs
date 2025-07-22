using LaYumba.Functional;
using MeanderingHeroes.Engine.Types.Considerations;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public delegate Utility GetConsideration(Entity pawn);

    public class ConsiderationContext
    {
        private readonly Game _game;
        public GameState StateSnapshot { get; private set; }
        public Blackboard Blackboard => _game.Blackboard;

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
            { ConsiderationType: ConsiderationType.PawnSpeed } => pawn => Math.Clamp(pawn.Speed, 0f, 1f),
            { ConsiderationType: ConsiderationType.PawnAvarice } => PawnAvarice,
            { ConsiderationType: ConsiderationType.Hunger } => PawnHunger,
            { ConsiderationType: ConsiderationType.FoodSupply } => FoodSupply,
            { ConsiderationType: ConsiderationType.ForageFoodDistance } => ForageFoodDistance,
            DecisionOnHex { ConsiderationType: ConsiderationType.HexDistance, Target: var hex } => DistanceToHex(hex),
            _ => throw new ArgumentException($"Unexpected ConsiderationType: {forDecision.ConsiderationType.ToString()}")
        };
        public static GetConsideration FoodSupply => pawn => pawn.FoodSupply / 5f;
        public static GetConsideration PawnHunger => pawn => pawn.Hunger;
        public static GetConsideration PawnAvarice => _ => 0.5f; //Math.Clamp(entity.Greed * 1000f / entity.Wealth, 0f, 1f);
        public static GetConsideration DistanceToHex(FractionalHex hex) => pawn
            => Math.Clamp(hex.Distance(pawn.HexCoords) / 10f, 0f, 1f); // anything over 10 hexes away is considered 1.0
        public static GetConsideration DistanceToTarget(Entity target) => DistanceToHex(target.HexCoords);

        public Consideration ForageFoodDistance => new ForageFoodDistance(this);
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
    public abstract class StatelessConsideration
    {
        public abstract GetConsideration Get {get;}
        public static implicit operator GetConsideration(StatelessConsideration consideration) => consideration.Get;
    }
    public abstract class Consideration : StatelessConsideration
    {
        protected readonly ConsiderationContext _context;
        public Consideration(ConsiderationContext context)
        {
            _context = context;
        }
    }
}

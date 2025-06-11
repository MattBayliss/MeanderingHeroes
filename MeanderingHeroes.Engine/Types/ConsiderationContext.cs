using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public delegate Utility GetConsideration(Entity pawn);

    public class ConsiderationContext
    {
        private readonly Game _game;
        private GameState _stateSnapshot;

        public ConsiderationContext(Game game)
        {
            _game = game;
            _stateSnapshot = _game.GameState;
        }
        public void SetStateSnapshot()
        {
            _stateSnapshot = _game.GameState;
        }
        public GetConsideration GetConsideration(Decision forDecision) => forDecision switch
        {
            { ConsiderationType: ConsiderationType.PawnSpeed } => entity => entity is Entity pawn ? Math.Clamp(pawn.Speed, 0f, 1f) : 0f,
            { ConsiderationType: ConsiderationType.PawnAvarice } => PawnAvarice,
            { ConsiderationType: ConsiderationType.Hunger } => PawnHunger,
            { ConsiderationType: ConsiderationType.FoodSupply } => FoodSupply,
            { ConsiderationType: ConsiderationType.ForageFoodDistance } => ForageFoodDistance(),
            DecisionOnHex { ConsiderationType: ConsiderationType.HexDistance, Target: var hex } => DistanceToHex(hex),
            DecisionOnEntity { ConsiderationType: ConsiderationType.TargetDistance, Target: var target } => DistanceToTarget(target),
            _ => _ => 0f
        };
        public static GetConsideration FoodSupply => pawn => pawn.FoodSupply / 5f;
        public static GetConsideration PawnHunger => pawn => pawn.Hunger;
        public static GetConsideration PawnAvarice => _ => 0.5f; //Math.Clamp(entity.Greed * 1000f / entity.Wealth, 0f, 1f);
        public static GetConsideration DistanceToHex(FractionalHex hex) => pawn
            => Math.Clamp(hex.Distance(pawn.HexCoords) / 10f, 0f, 1f); // anything over 10 hexes away is considered 1.0
        public static GetConsideration DistanceToTarget(Entity target) => DistanceToHex(target.HexCoords);

        // TODO: make Considerations command classes to better encapsulate each Consideration context and blackboard data
        public GetConsideration ForageFoodDistance() {

            var getClosestFoodItem = (Entity pawn) => _stateSnapshot
                .FoodItems
                .Select(fi => (FoodItem: fi, Distance: pawn.HexCoords.Distance(fi.HexCoords)))
                .OrderByDescending(fi => fi.Distance)
                .Head();

            return pawn =>
            {
                var hex = pawn.HexCoords.Round();

                var bbKey = BlackboardKeys.ForageFoodDistance(hex);

                // looks for a value for food distance already on the blackboard, otherwise calculates
                // distance and returns a new record to add to the blackboard
                (var cValue, var bbItem) = _game.Blackboard.Get(bbKey)
                    .Match(
                        None: () => getClosestFoodItem(pawn).Match(
                            None: () => (0f, Some(new ForageFoodLocation(hex, 0f))),
                            Some: fi => (fi.Distance, Some(new ForageFoodLocation(fi.FoodItem.HexCoords, fi.Distance)))
                            ),
                        Some: bbValue => (bbValue.Value, None));

                // if there's an item to add to the Blackboard, do it
                bbItem.ForEach(bb => _game.Blackboard.Set(bbKey, bb));

                return cValue;
            };
        }
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

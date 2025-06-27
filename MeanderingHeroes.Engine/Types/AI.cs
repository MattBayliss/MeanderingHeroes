using LaYumba.Functional;

namespace MeanderingHeroes.Engine.Types
{
    /// <summary>
    /// Game => Entity => (Dse, Command)
    /// </summary>
    /// <param name="game"></param>
    /// <param name="pawn"></param>
    /// <returns></returns>
    public delegate Func<Entity, Behaviour> BehaviourTemplate(Game game);
    public record Behaviour(Dse Dse, Command Command);
    [Flags]
    public enum DseStatus
    {
        Unknown = 0b000,
        Running = 0b001,
        Completed = 0b010,
        Aborted = 0b110
    }
    /// <summary>
    /// (Option<Entity> EntityChange, DseStatus Status)
    /// </summary>
    public record AiResult(Option<Entity> EntityChange, DseStatus Status);
    /// <summary>
    /// (Entity, GameState) => (Option<Entity>, DseStatus)
    /// </summary>
    public delegate AiResult Command(Entity entity, GameState state);


    /// <summary>
    /// Decision Score Evaluator
    /// </summary>
    public readonly record struct Dse
    {
        public static int lastId = 0;
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public float Weight { get; init; } = 1f;
        public float Inertia { get; init; } = 0f;
        public ImmutableList<Decision> Decisions { get; init; }

        public Dse(string name, string description, float weight, IEnumerable<Decision> decisions)
        {
            Id = lastId++;
            Name = name;
            Description = description;
            Weight = weight;
            Decisions = decisions.ToImmutableList();
        }
    }

    public record EntityBehaviour(int EntityId, int DseId, float Inertia, Command Run);

    public readonly record struct CurveParams(float M, float K, float B, float C);

    public readonly record struct CurveDefinition(string Description, CurveType CurveType, CurveParams CurveParams) {
        public CurveDefinition(CurveType curveType, float m, float k, float b, float c)
        : this($"[{curveType.ToString()}, {m}, {k}, {b}, {c}]", curveType, new CurveParams(m, k, b, c)) { }
        public CurveDefinition(string description, CurveType curveType, float m, float k, float b, float c) 
        : this(description, curveType, new CurveParams(m, k, b, c)) { }
    }
    public record Decision(ConsiderationType ConsiderationType, CurveDefinition Curve);
    public record DecisionOnHex : Decision
    {
        public FractionalHex Target { get; init; }
        public DecisionOnHex(ConsiderationType considerationType, CurveDefinition curve, FractionalHex targetHex) : base(considerationType, curve)
        {
            Target = targetHex;
        }
    }
    public record DecisionOnEntity : Decision
    {
        public Entity Target { get; init; }
        public DecisionOnEntity(ConsiderationType considerationType, CurveDefinition curve, Entity target) : base(considerationType, curve)
        {
            Target = target;
        }
    }
    public record Consideration;
    public enum CurveType
    {
        Quadratic,
        Logistic,
        Step
    }
    public readonly record struct ResponseCurve(CurveType curveType, CurveParams curveParams);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public delegate Utility CurveFunction(Utility x);
    public delegate CurveFunction CurveFunctionBuilder(CurveParams curveParams);

    public record Agent(int Id, ImmutableList<Dse> DSEs);

    public static class CurveFunctions
    {
        public static CurveFunction ToFunc(this CurveDefinition cd) =>
            cd switch
            {
                { CurveType: CurveType.Quadratic } => Quadratic(cd.CurveParams),
                { CurveType: CurveType.Logistic } => Logistic(cd.CurveParams),
                { CurveType: CurveType.Step} => Step(cd.CurveParams),
                _ => throw new ArgumentException($"Unexpected CurveType: {cd.CurveType.ToString()}")

            };
        /// <summary>
        /// Define a quadratic or linear curve (linear curve is when k = 1)
        /// x: the parameter being passed into the linear curve function</param>
        /// m: slope
        /// k: exponent
        /// b: y-intercept (vertical shift)</param>
        /// c: x-intercept (horizontal shift)</param>
        /// </summary>
        public static CurveFunctionBuilder Quadratic => cp => x => cp.M * MathF.Pow(x - cp.C, cp.K) + cp.B;
        /// <summary>
        /// m = slope of the line at inflection point
        /// k = vertical size of the curve
        /// b = y-intercept (vertical shift)
        /// c = x-intercept of the inflection point (horizontal shift)
        /// </summary>
        public static CurveFunctionBuilder Logistic => cp => x => cp.B + cp.K / (1f + cp.M * MathF.Exp(-10f * (x - cp.C)));
        /// <summary>
        /// m = value when x is less than b
        /// k = value when x is greater or equal to b
        /// b = the inflection point
        /// c = ignored
        /// </summary>
        public static CurveFunctionBuilder Step => cp => x => x < cp.B ? cp.M : cp.K;
    }
}

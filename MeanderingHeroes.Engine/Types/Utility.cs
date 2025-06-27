namespace MeanderingHeroes.Engine.Types
{
    /// <summary>
    /// A value struct to constrain utility values between 0f and 1f
    /// </summary>
    public readonly record struct Utility
    {
        private float Value { get; init; } 
        private Utility(float value)
        {
            Value = Math.Clamp(value, 0f, 1f);
        }
        public static implicit operator float(Utility utility) => utility.Value;
        public static implicit operator Utility(float value) => new Utility(value);
        public static implicit operator string(Utility utility) => utility.ToString();
        public override string ToString() => $"{Value:F3}";
        public override int GetHashCode() => Value.GetHashCode();
    }
}

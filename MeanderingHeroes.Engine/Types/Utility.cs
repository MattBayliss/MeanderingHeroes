using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (value > 1f || value < 0f)
            {
                throw new ArgumentOutOfRangeException("value", value, "Utility value must be between 0.0f and 1.0f");
            }
            Value = value;
        }

        public static implicit operator float(Utility utility) => utility.Value;
        public static implicit operator Utility(float value) => new Utility(value);
        public static implicit operator string(Utility utility) => utility.ToString();
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();
    }
}

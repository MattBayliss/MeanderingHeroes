using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    /// <summary>
    /// A Value type representing a skill. Untrained skills are 0, up to
    /// a value of 4.0 representing legendary aptitude
    /// </summary>
    /// <param name="Value"></param>
    public readonly record struct Skill(float Value)
    {
        public static implicit operator float(Skill skill) => skill.Value;
        public static implicit operator Skill(float value) => new Skill(value);
        public static implicit operator string(Skill skill) => skill.ToString();
        public override string ToString() => $"{Value:F3}";
        public override int GetHashCode() => Value.GetHashCode();
    }
}

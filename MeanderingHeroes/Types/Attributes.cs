using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types
{
    public abstract record Attribute
    {
        public int Score { get; init; }
        public Attribute(int score)
        {
            if(!IsValid(score))
            {
                throw new ArgumentException($"Invalid attribute value: {score}. Attributes must be between 3 and 18");
            }
            Score = score;
        }
        private static bool IsValid(int attribute) => attribute >= 3 && attribute <= 18;
    }

    public record Strength : Attribute
    {
        public Strength(int str) : base(str) { }
    }
    public record Dexterity : Attribute
    {
        public Dexterity(int dex) : base(dex) { }
    }
    public record Willpower : Attribute
    {
        public Willpower(int wil) : base(wil) { }
    }

    public abstract record Trait;
    public record Openness : Trait;
    public record Conscientiousness : Trait;
    public record Extroversion : Trait;
    public record Agreeableness : Trait;
    public record Neuroticism : Trait;
}

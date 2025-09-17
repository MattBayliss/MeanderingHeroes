using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types.Skills
{
    public delegate Utility SkillFunction(Entity agent);
    internal static class PlayerSkills
    {
        // TODO: factor in different food types
        public static SkillFunction ChanceOfFindingFood(FoodType foodType) => agent 
            => Math.Clamp(agent.Survival.Value + 0.1f, 0.1f, 0.98f);
    }
}

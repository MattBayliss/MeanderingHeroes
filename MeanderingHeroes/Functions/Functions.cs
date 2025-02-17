using MeanderingHeroes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LaYumba.Functional.F;

namespace MeanderingHeroes
{
    public static partial class Functions
    {
        public static Hero AddGoal(this Hero hero, Goal goal) => hero with { Goals = hero.Goals.Add(goal) };
    }
}

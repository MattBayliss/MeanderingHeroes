using MeanderingHeroes.Engine.Types.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public static class ConsideratonsLibrary
    {
        public static ConsiderationD AcquireWealth => (_, entity) => entity switch
        {
            SmartEntity se => Some(0.5f), //TODO: Calculate based off of entity's attributes and current wealth?
            _ => None
        };
    }
}

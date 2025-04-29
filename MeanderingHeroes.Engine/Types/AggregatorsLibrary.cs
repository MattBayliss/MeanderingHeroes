using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public static class AggregatorsLibrary
    {
        public static Aggregator Average = utilities => utilities.Average(u => (float)u);
    }
}

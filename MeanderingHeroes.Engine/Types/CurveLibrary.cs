using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public static class CurveLibrary
    {
        public static CurveDefinition BasicLinear => new(CurveType.Quadratic, 1, 1, 0, 0);
        public static CurveDefinition NegativeLinear => new(CurveType.Quadratic, -1, 1, 1, 0);
    }
}

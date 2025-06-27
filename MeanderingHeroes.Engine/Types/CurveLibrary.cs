using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public static class CurveLibrary
    {
        public static CurveDefinition BasicLinear => new("BasicLinear", CurveType.Quadratic, 1f, 1f, 0f, 0f);
        public static CurveDefinition NegativeLinear => new("NegativeLinear", CurveType.Quadratic, -1f, 1f, 1f, 0f);
        public static CurveDefinition BasicLogistic => new("BasicLogistic", CurveType.Logistic, 1f, 1f, 0f, 0.5f);
        public static CurveDefinition SupplyLogistic => new("SupplyLogistic", CurveType.Logistic, 1f, 1.1f, -0.02f, 0.5f);
        public static CurveDefinition ReverseLogistic => new("ReverseLogistic", CurveType.Logistic, 1f, -1f, 1f, 0.5f);
        public static CurveDefinition LogisticTrailOff => new("LogisticTrailOff", CurveType.Logistic, 1f, -1f, 1.05f, 0.3f);
        public static CurveDefinition WithinInteractionRange => new("WithinInteractionRange", CurveType.Step, 1f, 0f, 0.05f, 0f);
        public static CurveDefinition NotUrgentUntilItIs => new("NotUrgentUntilItIs", CurveType.Logistic, 1f, 1.2f, 0f, 0.7f);
    }
}

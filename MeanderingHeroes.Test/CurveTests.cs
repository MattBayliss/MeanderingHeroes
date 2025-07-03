using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Test
{
    public class CurveTests
    {
        [Fact]
        public void PositiveCurvesGoUp()
        {
            List<CurveDefinition> curvesThatGoUp = [
                CurveLibrary.BasicLinear,
                CurveLibrary.BasicLogistic,
                CurveLibrary.NotUrgentUntilItIs
            ];

            var utilities = Range(0, 10).Select(i => (Utility)(0.1f * i));

            foreach (var curve in curvesThatGoUp)
            {
                var results = utilities.Select(u => curve.ToFunc()(u));
                var resultPairs = results.Zip(results.Skip(1));
                foreach (var pair in resultPairs)
                {
                    Assert.True(pair.Second >= pair.First);
                }
            }
        }

        [Fact]
        public void NegativeCurvesGoDown()
        {
            List<CurveDefinition> curvesThatGoDown = [
                CurveLibrary.NegativeLinear,
                CurveLibrary.ReverseLogistic,
                CurveLibrary.LogisticTrailOff,
                CurveLibrary.WithinInteractionRange
            ];

            var utilities = Range(0, 10).Select(i => (Utility)(0.1f * i));

            foreach (var curve in curvesThatGoDown)
            {
                var results = utilities.Select(u => curve.ToFunc()(u));
                var resultPairs = results.Zip(results.Skip(1));
                foreach (var pair in resultPairs)
                {
                    Assert.True(pair.Second <= pair.First);
                }
            }
        }
        
        [Fact]
        public void SupplyLogisticIsZeroWhenSupplyIsZero()
        {
            var supplyLogistic = CurveLibrary.SupplyLogistic.ToFunc();
            Assert.Equal((Utility)0f, supplyLogistic(0f));
        }
    }
}

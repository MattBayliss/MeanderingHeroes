using MeanderingHeroes.Engine;
using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Test
{
    public class CoordinateTests
    {
        [Fact]
        public void RMinusOneTest()
        {
            var hexZero = new Hex(0, 0);
            var offsetZero = new Point(0, 0);
            Assert.Equal(offsetZero, hexZero.Centre());

            // https://www.redblobgames.com/grids/hexagons/#conversions-offset
            var rMinus1 = new Hex(0, -1);
            Point rMinus1Offset = Vector2.Normalize(new Vector2(-1, -MathF.Sqrt(3)));
            var rMinus1HexCentre = rMinus1.Centre();

            Assert.Equal(rMinus1Offset.X, rMinus1HexCentre.X, 0.001f);
            Assert.Equal(rMinus1Offset.Y, rMinus1HexCentre.Y, 0.001f);
        }
    }
}

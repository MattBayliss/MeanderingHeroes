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
        public void HexWidthIs1()
        {
            var offsetZero = Vector2.Zero;
            Transforms.Init(offsetZero, 1f, 2f / MathF.Sqrt(3));
            var hexZero = new Hex(0, 0);
            Assert.Equal(offsetZero, hexZero.Centre());

            // https://www.redblobgames.com/grids/hexagons/#conversions-offset
            var qPlus1 = new Hex(1, 0);
            var qPlus1Offset = new Vector2(1f, 0);
            var qPlus1HexCentre = qPlus1.Centre();

            Assert.Equal(qPlus1Offset.X, qPlus1HexCentre.X);
            Assert.Equal(qPlus1Offset.Y, qPlus1HexCentre.Y);
        }
        [Fact]
        public void RMinusOneTest()
        {
            var offsetZero = Vector2.Zero;
            Transforms.Init(offsetZero, 1f, 2f / MathF.Sqrt(3)); 
            var hexZero = new Hex(0, 0);
            Assert.Equal(offsetZero, hexZero.Centre());

            // https://www.redblobgames.com/grids/hexagons/#conversions-offset
            var rMinus1 = new Hex(0, -1);
            var rMinus1Offset = Vector2.Normalize(new Vector2(-1, -MathF.Sqrt(3)));
            var rMinus1HexCentre = rMinus1.Centre();

            Assert.Equal(rMinus1Offset.X, rMinus1HexCentre.X, 0.001f);
            Assert.Equal(rMinus1Offset.Y, rMinus1HexCentre.Y, 0.001f);
        }
        [Fact]
        public void TransformToAndFromAxialCoordsWithNoOffset()
        {
            var hexOriginOffsetInGame = Vector2.Zero;

            Transforms.Init(
                hexOriginOffsetInGame: hexOriginOffsetInGame,
                hexWidthInGame: 29f,
                hexHeightInGame: 33f);


            // Zero hex should equate to zero
            Assert.Equal(hexOriginOffsetInGame, Transforms.Instance.ToVector2(new FractionalHex(0, 0)));
            // Zero point should equate to Hex 0,0
            Assert.Equal(new FractionalHex(0, 0), Transforms.Instance.ToAxial(Vector2.Zero));

            //                       q
            Assert.Equal(new Vector2(29f, 0), Transforms.Instance.ToVector2(new FractionalHex(1f, 0)));

            Assert.Equal(new Vector2(29f / 2f, 33f * 0.75f), Transforms.Instance.ToVector2(new FractionalHex(0, 1f)));
        }
        [Fact]
        public void TransformToAndFromAxialCoordsWithOffset()
        {
            var hexOriginOffsetInGame = new Vector2(14.5f, 16.5f);

            Transforms.Init(
                hexOriginOffsetInGame: hexOriginOffsetInGame,
                hexWidthInGame: 29f,
                hexHeightInGame: 33f);


            // Zero hex should equate to offset
            Assert.Equal(hexOriginOffsetInGame, Transforms.Instance.ToVector2(new FractionalHex(0, 0)));

            // offset should equate to Hex 0,0
            Assert.Equal(new Hex(0, 0), Transforms.Instance.ToAxial(hexOriginOffsetInGame).Round());

            // from Godot tests, I know Hex(4,1) == Vector2 (145, 41.25)

            //                       q
            Assert.Equal(new Vector2(145f, 41.25f), Transforms.Instance.ToVector2(new FractionalHex(4f, 1f)));

        }
    }
}

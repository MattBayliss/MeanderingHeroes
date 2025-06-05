using System.Numerics;

namespace MeanderingHeroes.Engine.Types
{
    public class Transforms
    {
        private static float Sqrt3 = MathF.Sqrt(3f);
        private Matrix3x2 _toAxial { get; set; }
        private Matrix3x2 _toCartesian { get; set; }

        public Transforms(Vector2 hexOriginOffsetInGame, float hexWidthInGame, float hexHeightInGame)
        {
            // X: _hexWidth * (hex.Q + hex.R / 2f),
            var refX = new Vector2(1f, 0.5f) * hexWidthInGame;
            // Y: hexHeight * 0.75
            var refY = new Vector2(0, hexHeightInGame * 0.75f);

            var toCartesian = new Matrix3x2(refX.X, refY.X, refX.Y, refY.Y, hexOriginOffsetInGame.X, hexOriginOffsetInGame.Y);
            if (Matrix3x2.Invert(toCartesian, out var toAxial))
            {
                _toAxial = toAxial;
                _toCartesian = toCartesian;
            }
            else
            {
                throw new ArgumentException($"Failed to generate the inverse transform - check supplied parameters: {{ hexOriginOffsetInGame: {hexOriginOffsetInGame}, hexWidthInGame: {hexWidthInGame}, hexHeightInGame: {hexHeightInGame} }}");
            }

        }

        public Vector2 ToGameXY(FractionalHex axial) => Vector2.Transform((Vector2)axial, _toCartesian);
        public FractionalHex ToHexQR(Vector2 gameCoords) => (FractionalHex)Vector2.Transform(gameCoords, _toAxial);
    }
}

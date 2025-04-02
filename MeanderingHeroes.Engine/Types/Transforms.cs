using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public class Transforms
    {
        private static float Sqrt3 = MathF.Sqrt(3f);
        private Matrix3x2 _toAxial { get; set; }
        private Matrix3x2 _toCartesian { get; set; }
        private static Lazy<Transforms>? _instance = null;

        public static bool Initialised => _instance != null;

        public static void Init(Vector2 hexOriginOffsetInGame, float hexWidthInGame, float hexHeightInGame)
        {
            // X: _hexWidth * (hex.Q + hex.R / 2f),
            var refX = new Vector2(1f, 0.5f) * hexWidthInGame;
            // Y: hexHeight * 0.75
            var refY = new Vector2(0, hexHeightInGame * 0.75f);

            var toCartesian = new Matrix3x2(refX.X, refY.X, refX.Y, refY.Y, hexOriginOffsetInGame.X, hexOriginOffsetInGame.Y);
            if (Matrix3x2.Invert(toCartesian, out var toAxial))
            {
                _instance = new Lazy<Transforms>(() => new Transforms { _toAxial = toAxial, _toCartesian = toCartesian });
            }
            else
            {
                throw new ArgumentException($"Failed to generate the inverse transform - check supplied parameters: {{ hexOriginOffsetInGame: {hexOriginOffsetInGame}, hexWidthInGame: {hexWidthInGame}, hexHeightInGame: {hexHeightInGame} }}");
            }

        }

        public static Transforms Instance
        {
            get
            {
                {
                    if (_instance == null) throw new Exception("Attempted to use Transforms.Instance before Transforms.Init was called");

                    return _instance!.Value;
                }
            }
        }

        public Vector2 ToVector2(FractionalHex axial) => Vector2.Transform((Vector2)axial, _toCartesian);
        public FractionalHex ToAxial(Vector2 gameCoords) => (FractionalHex)Vector2.Transform(gameCoords, _toAxial);
    }
}

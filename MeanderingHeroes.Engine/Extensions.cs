using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine
{
    public static partial class Extensions
    {
        public static Vector2 ToGameXY(this Game game, FractionalHex fhex) => game.Transforms.ToGameXY(fhex);
        public static Vector2 HexCentreXY(this Game game, Hex hex) => game.Transforms.ToGameXY(hex);
        public static Vector2 HexCentreXY(this Game game, FractionalHex fhex) => game.Transforms.ToGameXY(fhex.Round());
        public static FractionalHex HexQR(this Game game, Vector2 gameXY) => game.Transforms.ToHexQR(gameXY);
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if(!dict.TryAdd(key, value))
            {
                dict[key] = value;
            }
        }
    }
}

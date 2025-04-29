using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
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
        public static ImmutableDictionary<TKey, ImmutableList<TItem>> AddOrUpdate<TKey, TItem>(this ImmutableDictionary<TKey, ImmutableList<TItem>> dict, TKey key, TItem value) where TKey : struct
            => dict.Lookup(key).Match(
                    None: () => dict.Add(key, [value]),
                    Some: (existingValues) => dict.SetItem(key, existingValues.Add(value))
                );
    }
}

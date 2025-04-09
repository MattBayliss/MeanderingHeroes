using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Components
{
    public static class UtilityLibrary
    {
        /// <summary>
        /// For a player-set course of action the urgency will decrease over time, unless the player keeps renewing it. Each
        /// call will assume it's a new tick, so ONLY CALL ONCE PER GAME LOOP
        /// </summary>
        /// <param name="decayOverTicks">The curve function that outputs a result between 0.0 and 1.0</param>
        /// <returns></returns>
        public static UtilityDelegate PlayerUtility(Func<int, Utility> decayOverTicks)
        {
            int tick = 0;

            return (_, _) => decayOverTicks(tick++);
        }
        /// <summary>
        /// A generic function to clamp a UtilityDelegate function between two values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="curveFunc"></param>
        /// <param name="max">The maximum utility it can have (provided it's between 0.0 and 1.0)</param>
        /// <param name="min">The mimumum utility it can have (provided it's between 0.0 and 1.0, and less than Max)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Func<T, Utility> ClampedUtilityFunction<T>(this Func<T, Utility> curveFunc, Utility max, Utility min)
        {
            if (min > max)
            {
                throw new ArgumentException("min", $"min ({min}) must be less than max ({max})");
            }
            return tinput => min + curveFunc(tinput) * (max - min);
        }
            

        public static UtilityDelegate DistanceUtility(Entity targetEntity, Func<float, Utility> utilityForDistance)
        {
            var entityId = targetEntity.Id;
            Func<IEnumerable<Entity>, Option<FractionalHex>> coordsForEntity = entities 
                => entities
                    .Find(entity => entity.Id == entityId)
                    .Map(entity => entity.HexCoords);

            return (game, entity) => coordsForEntity(game.Entities).Match(
                None: () => (Utility)0f,
                Some: (targetCoords) => utilityForDistance(entity.HexCoords.Distance(targetCoords))
            );
        }
    }
}

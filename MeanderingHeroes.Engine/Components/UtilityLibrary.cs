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
        /// For a player set course of action the urgency will decrease over time, unless the player keeps renewing it. Each
        /// call will assume it's a new tick, so ONLY CALL ONCE PER GAME LOOP
        /// </summary>
        /// <param name="max">The maximum utility it can have (provided it's between 0.0 and 1.0)</param>
        /// <param name="min">The mimumum utility it can have (provided it's between 0.0 and 1.0, and less than Max)</param>
        /// <param name="decayOverTicks">The curve between function that outputs a result between 0.0 and 1.0</param>
        /// <returns></returns>
        public static UtilityDelegate PlayerUtility(Utility max, Utility min, Func<int, Utility> decayOverTicks)
        {
            if (min > max)
            {
                throw new ArgumentException("min", $"min ({min}) must be less than max ({max})");
            }



            Func<Utility, Utility> clampedFunc = input => min + input * (max - min);

            int tick = 0;

            return (_, _) => clampedFunc(decayOverTicks(tick++));
        }
    }
}

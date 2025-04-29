using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public static class BehavioursLibrary
    {
        public static Behaviour Acquire(Game game, Entity target, string name, float value)
            => PathFinding.GeneratePathGoalBehaviour(game, InteractionsLibrary.DesirabilityOverDistance(value), target.Hex);
    }
}
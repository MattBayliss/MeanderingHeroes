using MeanderingHeroes.Engine.Components;
using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.DecisionScoreEvaluators
{
    public static class Library
    {
        public static (Dse Behaviour, BehaviourDelegate BehaviourFunc) DestinationBehaviour(Game game, Entity pawn, Hex destination)
        {
            var goToDestination = new DecisionOnHex(
                ConsiderationType.TargetDistance,
                new CurveDefinition(CurveType.Quadratic, -1, 2f, 1.02f, 0),
                destination);

            var playerDestinationBehaviour = new Dse(
                name: "DestinationBehaviour",
                description: "player set destination",
                weight: 1f,
                decisions: [goToDestination]);

            return ( playerDestinationBehaviour, PathFinding.GeneratePathGoalBehaviour(game, pawn.Hex, destination));
        }
    }
}

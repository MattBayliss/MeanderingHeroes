using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types.Behaviours
{
    public static partial class BehavioursLibrary
    {
        public static BehaviourTemplate PlayerSetDestination(FractionalHex destination)
            => game => pawn => new(
                PlayerSetDestinationDSE(destination),
                PathFinding.GeneratePathGoalBehaviour(
                    game: game, 
                    start: pawn.HexCoords, 
                    end: destination, 
                    statusFunc: entity => entity.HexCoords == destination ? DseStatus.Completed : DseStatus.Running
                    ));

        private static Dse PlayerSetDestinationDSE(FractionalHex destination)
        {
            var goToDestination = new DecisionOnHex(
                ConsiderationType.HexDistance,
                CurveLibrary.LogisticTrailOff,
                destination);

            return new Dse(
                name: "PlayerSetDestination",
                description: "player set destination",
                weight: 1f,
                decisions: [goToDestination]);
        }
    }
}

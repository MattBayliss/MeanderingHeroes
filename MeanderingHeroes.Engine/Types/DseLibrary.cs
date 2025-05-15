using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public static class DseLibrary
    {
        public static Dse PlayerSetDestinationDSE(Hex destination)
        {
            var goToDestination = new DecisionOnHex(
                ConsiderationType.TargetDistance,
                new CurveDefinition(CurveType.Quadratic, -1, 2f, 1.02f, 0),
                destination);

            return new Dse(
                name: "PlayerSetDestination",
                description: "player set destination",
                weight: 1f,
                decisions: [goToDestination]);
        }
    }
}
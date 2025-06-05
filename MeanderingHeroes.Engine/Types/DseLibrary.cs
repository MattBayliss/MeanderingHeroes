using static LaYumba.Functional.F;

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

        public static Dse MoveToForageFood()
        {
            // closest food source - food vendor, foraging, hunting,  farming
            // decisions: food scarcity, food source availabilty
            var decisions = List(
                new Decision(ConsiderationType.Hunger, CurveLibrary.BasicLinear),
                new Decision(ConsiderationType.FoodSupply, CurveLibrary.NegativeLinear),

                // TODO: Needs to be 0 when we're close enough so a "Forage" DSE can take over
                new Decision(ConsiderationType.ForageFoodDistance, CurveLibrary.BasicLinear) 
            );

            return new Dse(
                name: "ForageForFood",
                description: "forage for nearby food",
                weight: 2f,
                decisions: decisions
                );
        }
    }
}
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public static class ConsideratonsLibrary
    {
        public static ConsiderationD AcquireWealth => (_, agent, target) => agent switch
        {
            SmartEntity => Some(0.5f), //TODO: Calculate based off of agent's attributes and current wealth?
            _ => None
        };
        public static ConsiderationD Nearby => (_, agent, target) => Some(target.HexCoords.Distance(agent.HexCoords));
    }
}

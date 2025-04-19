using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Types
{
    public static class ConsideratonsLibrary
    {
        public static ConsiderationD AcquireWealth => (_, entity) => entity switch
        {
            SmartEntity se => Some(0.5f), //TODO: Calculate based off of entity's attributes and current wealth?
            _ => None
        };
    }
}

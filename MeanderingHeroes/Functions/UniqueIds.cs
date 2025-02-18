namespace MeanderingHeroes
{
    
    public static class UniqueIds
    {
        private static long _commandId = 0;
        private static int _doerId = 0;

        public static int NextDoerId => _doerId++;
        public static long NextCommandId => _commandId++;
    }

}

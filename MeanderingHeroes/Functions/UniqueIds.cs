using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Functions
{
    public static class UniqueIds
    {
        private static long _commandId = 0;
        private static int _doerId = 0;

        public static int NextDoerId => _doerId++;
        public static long NextCommandId => _commandId++;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types.Commands
{
    public abstract record DoerIntent : Command
    {
        public int DoerId { get; init; }
        public DoerIntent(int doerId) : base()
        {
            DoerId = doerId;
        }
    }
}

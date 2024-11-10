using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types.Commands
{
    public record HuntIntent : DoerIntent
    {
        public HuntIntent(int doerId) : base(doerId)
        {
        }
    }
}

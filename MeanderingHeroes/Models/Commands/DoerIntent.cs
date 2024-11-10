using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Models.Commands
{
    public abstract class DoerIntent : Command
    {
        public int DoerId { get; init; }
        public DoerIntent(int doerId) : base()
        {
            DoerId = doerId;
        }
        public override bool Equals(Command? x, Command? y) =>
            (x, y) switch
            {
                (null, null) => true,
                (null, _) => false,
                (_, null) => false,
                (DoerIntent dx, DoerIntent dy) => (dx.DoerId, dx.Id) == (dy.DoerId, dy.Id),
                _ => false
            };

        public override int GetHashCode() => (DoerId, Id).GetHashCode();
    }
}

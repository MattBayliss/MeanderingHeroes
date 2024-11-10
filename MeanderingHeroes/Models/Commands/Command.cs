using MeanderingHeroes.Models.Doers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Models.Commands
{
    public class Command : IEqualityComparer<Command>
    {
        public virtual bool Equals(Command? x, Command? y) => x?.Id == y?.Id;
        public virtual int GetHashCode([DisallowNull] Command obj) => obj.Id.GetHashCode();
        public virtual Func<GameState, (GameState State, Events Events)> ProcessIntent { get; init; }

        public virtual long Id { get; init; }
        public Command()
        {
            Id = DateTime.UtcNow.Ticks;
            ProcessIntent = s => (s, []);
        }
    }
}

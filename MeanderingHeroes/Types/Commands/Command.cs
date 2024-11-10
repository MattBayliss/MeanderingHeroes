using MeanderingHeroes.Functions;
using MeanderingHeroes.Types.Doers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types.Commands
{
    public record Command : IEqualityComparer<Command>
    {
        public virtual bool Complete { get; set; } = false;
        public virtual bool Equals(Command? x, Command? y) => x?.Id == y?.Id;
        public virtual int GetHashCode([DisallowNull] Command obj) => obj.Id.GetHashCode();
        public virtual Func<GameState, (GameState State, Events Events)> ProcessIntent { get; init; }

        public virtual long Id { get; init; }
        public Command()
        {
            Id = UniqueIds.NextCommandId;
            ProcessIntent = s => (s, []);
        }
    }
}

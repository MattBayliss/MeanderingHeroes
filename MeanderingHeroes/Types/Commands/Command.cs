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
    public abstract record Command : IEqualityComparer<Command>
    {
        public virtual bool Equals(Command? x, Command? y) => x?.Id == y?.Id;
        public virtual int GetHashCode([DisallowNull] Command obj) => obj.Id.GetHashCode();
        /// <summary>
        /// Each derived Command should override this to make changes to the state and generate
        /// Events.
        /// TODO: Make this 'abstract' when I've gotten further along
        /// </summary>
        protected virtual Func<GameState, (GameState State, Events Events)> _runCommand { get; init; }

        /// <summary>
        /// A wrapper function to perform the _runCommand for the derived Command, and remove the
        /// Command from the GameState if it is completed (raises an EndEvent)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public (GameState State, Events Events) ProcessCommand(GameState state)
        {
            var (nextState, events) = this._runCommand(state);
            nextState = events.OfType<EndEvent>().Any() 
                ? nextState with { Commands = nextState.Commands.Remove(this) } 
                : nextState;

            return (nextState, events);
        }

        public virtual long Id { get; init; }
        public Command()
        {
            Id = UniqueIds.NextCommandId;
            _runCommand = s => (s, []);
        }
    }
}

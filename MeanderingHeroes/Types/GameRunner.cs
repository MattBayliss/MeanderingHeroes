using MeanderingHeroes.Types.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types
{
    using GameEvents = (GameState State, Events Events);

    public class GameRunner : IDisposable
    {
        private ImmutableList<Action<Event>> _eventListeners;
        public GameRunner()
        {
            _eventListeners = [];
        }

        public static GameEvents RunTurn(GameState state)
        {
            return state.Commands.Aggregate(
                seed: (state, []),
                func: (GameEvents gameevents, Command command) =>
                {
                    var commandresult = command.ProcessCommand(gameevents.State);
                    return commandresult with
                    {
                        Events = gameevents.Events.AddRange(commandresult.Events)
                    };
                }
            );
        }


        public void Dispose()
        {
            _eventListeners = _eventListeners.Clear();
        }
    }
}

using MeanderingHeroes.Types.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Types
{
    public class GameRunner : IDisposable
    {
        private ImmutableList<Action<Event>> _eventListeners;
        public GameRunner()
        {
            _eventListeners = [];
        }

        public (GameState, Events) RunTurn(GameState state, ImmutableList<Command> commands)
        {
            return (state, []);
        }


        public void Dispose()
        {
            _eventListeners = _eventListeners.Clear();
        }
    }
}

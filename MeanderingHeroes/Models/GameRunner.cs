using MeanderingHeroes.Models.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Models
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

        }


        public void Dispose()
        {
            _eventListeners = _eventListeners.Clear();
        }
    }
}

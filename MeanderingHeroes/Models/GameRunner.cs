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
        private Queue<Action<Event>> _newListeners;
        public GameRunner()
        {
            _eventListeners = [];
        }



        public void Dispose()
        {
            _eventListeners = _eventListeners.Clear();
        }
    }
}

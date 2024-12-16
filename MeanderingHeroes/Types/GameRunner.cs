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
            // Commands => Events
            // Events => Reactions
            // Reactions => Interrupts
            // If an Event isn't interupted, the new State is accepted

            var reactions = state
                .Doers
                .SelectMany(doer => doer.Reactions)
                .SelectMany(reaction => reaction.Triggers)
                .Select<Trigger, Func<GameState, Event, GameEvents>>(trigger => (GameState gs, Event ev) => trigger switch
                {
                    ProximityTrigger pt => (gs, []),
                    _ => (gs, [])
                });

            return state.Commands.Aggregate(
                seed: (state, []),
                func: (GameEvents gameevents, Command command) =>
                {
                    var commandresult = command.ProcessCommand(gameevents.State);

                    reactions.Select(rf => )


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

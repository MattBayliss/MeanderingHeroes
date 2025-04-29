using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using static LaYumba.Functional.F;

namespace MeanderingHeroes.Engine.Components
{
    public class UtilityAIComponent : IComponent
    {
        private Dictionary<Hex, IEnumerable<Behaviour>> _behavioursByHex;
        private Dictionary<int, IEnumerable<Behaviour>> _bevavioursByEntityId;

        public UtilityAIComponent()
        {
            _behavioursByHex = new Dictionary<Hex, IEnumerable<Behaviour>>();
            _bevavioursByEntityId = new Dictionary<int, IEnumerable<Behaviour>>();
        }
        public void AddBehaviour(Entity entity, Behaviour behaviour)
        {
            _bevavioursByEntityId
                .Lookup(entity.Id)
                .Match(
                    None: () => _bevavioursByEntityId.Add(entity.Id, [behaviour]),
                    Some: existingBehaviours => _bevavioursByEntityId[entity.Id] = existingBehaviours.Append(behaviour)
                );
        }
        public void AddBehaviour(Hex hex, Behaviour behaviour)
        {
            _behavioursByHex
                .Lookup(hex)
                .Match(
                    None: () => _behavioursByHex.Add(hex, [behaviour]),
                    Some: existingBehaviours => _behavioursByHex[hex] = existingBehaviours.Append(behaviour)
                );
        }
        public GameState Update(Game game, GameState state)
        {
            var updatedEntities = state
                .Entities
                .Select(entity => entity switch
                {
                    SmartEntity agent => (Entity: entity, Updated: UpdateAgent(game, state.EntitiesInRange(agent), agent)),
                    _ => (entity, None)
                })
                .Select(eu => eu.Updated.Match(None: () => eu.Entity, Some: (updated) => updated));

            return new GameState(updatedEntities);
        }
        private Option<SmartEntity> UpdateAgent(Game game, IEnumerable<Entity> targets, SmartEntity agent)
            => targets
                .SelectMany(t => t.Behaviours.Select(b => (Target: t, Behaviour: b, Utility: b.Interaction.CalculateUtility(game, agent, t))))
                .OrderByDescending(tbu => tbu.Utility)
                // you should take the top 3 and randomly choose from those - depending
                // on the deviation of results
                // BUT this early on, just return the highest result every time
                .Head()
                .Map(tbu => tbu.Behaviour.Update(game, agent));

        private IEnumerable<Behaviour> BehavioursInRange(Hex ofHex, int hexRange = 1)
            => ofHex.HexesInRange(hexRange)
                .Bind(hex => _behavioursByHex.Lookup(hex))
                .Flatten();
    }

}

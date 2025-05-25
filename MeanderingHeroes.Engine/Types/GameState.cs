using LaYumba.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    using HexEntity = (Hex Hex, int EntityId);
    /// <summary>
    /// Should hold a snapshot of the "mutable" game parts - things that can change each turn.
    /// </summary>
    public record GameState
    {
        protected ImmutableHashSet<HexEntity> _hexEntities;
        public IEnumerable<Entity> Entities => _entitiesById.Values;
        public ImmutableDictionary<int, Dse> DseById { get; init; }
        public ImmutableHashSet<(int EntityId, int DseId, BehaviourDelegate Run)> Behaviours { get; init; }
        protected ImmutableDictionary<int, Entity> _entitiesById;
        
        public Option<Entity> this[int index] => _entitiesById.Lookup(index);

        public GameState(IEnumerable<Entity> entities)
        {
            Behaviours = [];
            DseById = ImmutableDictionary.Create<int, Dse>();
            _hexEntities = entities.Select(e => (e.HexCoords.Round(), e.Id)).ToImmutableHashSet();
            _entitiesById = entities.ToImmutableDictionary(e => e.Id, e => e);
        }

        public IEnumerable<Entity> EntitiesInRange(Entity ofEntity)
            => ofEntity.HexCoords.Neighbours()
                .SelectMany(hex => _hexEntities.Where(he => he.Hex == hex))
                .Bind(he => _entitiesById.Lookup(he.EntityId));

        public GameState AddEntity(Entity entity) => this with
        {
            _entitiesById = _entitiesById.SetItem(entity.Id, entity),
            _hexEntities = _entitiesById.Values.Select(e => (e.HexCoords.Round(), e.Id)).ToImmutableHashSet()
        };
        public GameState AddBehaviour(int entityId, Behaviour behaviour)
        {
            return this with {
                Behaviours = Behaviours.Add(new(entityId, behaviour.Dse.Id, behaviour.BehaviourFunc)),
                DseById = DseById.Add(behaviour.Dse.Id, behaviour.Dse)
            };
        }
        public GameState ModifyEntities(IEnumerable<Entity> entities)
        {
            return this with {
                _entitiesById = this._entitiesById.SetItems(entities.Select(e => new KeyValuePair<int, Entity>(e.Id, e))),
                _hexEntities = this._entitiesById.Values.Select(e => (e.HexCoords.Round(), e.Id)).ToImmutableHashSet()
            };
        }
        public GameState RemoveBehaviours(IEnumerable<int> DSEIds)
        {
            if(!DSEIds.Any()) { return this; }

            return this with
            {
                DseById = this.DseById.RemoveRange(DSEIds),
                Behaviours = this.Behaviours.ExceptBy(DSEIds, b => b.DseId).ToImmutableHashSet()
            };
        }
    }

}

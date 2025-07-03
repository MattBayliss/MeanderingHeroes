using LaYumba.Functional;

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
        public ImmutableHashSet<EntityBehaviour> Behaviours { get; init; }
        protected ImmutableDictionary<int, Entity> _entitiesById;
        public ImmutableHashSet<FoodItem> FoodItems { get; init; } = [];

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
                Behaviours = Behaviours.Add(new(entityId, behaviour.Dse.Id, 0f, behaviour.Command)),
                DseById = DseById.Add(behaviour.Dse.Id, behaviour.Dse)
            };
        }
        public GameState ModifyEntities(IEnumerable<Entity> entities)
        {
            var newEntitiesById = this._entitiesById.SetItems(entities.Select(e => new KeyValuePair<int, Entity>(e.Id, e)));
            return this with {
                _entitiesById = newEntitiesById,
                _hexEntities = newEntitiesById.Values.Select(e => (e.HexCoords.Round(), e.Id)).ToImmutableHashSet()
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
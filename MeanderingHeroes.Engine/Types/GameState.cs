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
        public Layer FoodItems { get; init; }
        public Option<Entity> this[int index] => _entitiesById.Lookup(index);
        public ImmutableList<TidbitLearnt> KnowledgeGained { get; init; }

        public GameState(IEnumerable<Entity> entities)
        {
            Behaviours = [];
            DseById = ImmutableDictionary.Create<int, Dse>();
            _hexEntities = entities.Select(e => (e.HexCoords.Round(), e.Id)).ToImmutableHashSet();
            _entitiesById = entities.ToImmutableDictionary(e => e.Id, e => e);
            FoodItems = new Layer([]);
            KnowledgeGained = [];
        }

        public IEnumerable<Entity> EntitiesInRange(Entity ofEntity)
            => ofEntity.HexCoords.Neighbours()
                .SelectMany(hex => _hexEntities.Where(he => he.Hex == hex))
                .Bind(he => _entitiesById.Lookup(he.EntityId));

        public Option<LayerItem> GetLayerItem(int layerId) => FoodItems.Items.Find(li => li.Id == layerId);
        public GameState AddEntity(Entity entity) => this with
        {
            _entitiesById = _entitiesById.SetItem(entity.Id, entity),
            _hexEntities = _entitiesById.Values.Select(e => (e.HexCoords.Round(), e.Id)).ToImmutableHashSet()
        };
        public GameState AddBehaviour(int entityId, Behaviour behaviour)
        {
            return this with
            {
                Behaviours = Behaviours.Add(new(entityId, behaviour.Dse.Id, 0f, behaviour.Command)),
                DseById = DseById.Add(behaviour.Dse.Id, behaviour.Dse)
            };
        }
        public GameState UpdateState(IEnumerable<StateChange> updates, IEnumerable<int> completedDSEIds)
        {            
            (var updatedLayerItems, var updatedEntityDict, var tidbitsLearnt) = updates.Aggregate(
                seed: (LayerItems: FoodItems.Items, EntitiesById: _entitiesById, TidbitsLearnt: ImmutableList.Create<TidbitLearnt>()),
                func: (acc, update) => update switch
                {
                    EntityChange ec => acc with { EntitiesById = acc.EntitiesById.SetItem(ec.UpdatedEntity.Id, ec.UpdatedEntity) },
                    LayerItemChange lic => acc with { LayerItems = acc.LayerItems.Replace(lic.OldItem, lic.NewItem) },
                    TidbitLearnt tbl => acc with { TidbitsLearnt = acc.TidbitsLearnt.Add(tbl) },
                    _ => acc
                });

            var dseById = this.DseById;
            var behaviours = this.Behaviours;

            if (completedDSEIds.Any())
            {
                dseById = dseById.RemoveRange(completedDSEIds);
                behaviours = behaviours.ExceptBy(completedDSEIds, b => b.DseId).ToImmutableHashSet();
            }

            return this with
            {
                FoodItems = new Layer(updatedLayerItems),
                _entitiesById = updatedEntityDict,
                _hexEntities = updatedEntityDict.Values.Select(e => (e.HexCoords.Round(), e.Id)).ToImmutableHashSet(),
                DseById = dseById,
                Behaviours = behaviours,
                KnowledgeGained = tidbitsLearnt
            };
        }
    }
}
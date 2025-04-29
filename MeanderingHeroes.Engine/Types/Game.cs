using MeanderingHeroes.Engine.Components;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using System.Runtime.InteropServices;

namespace MeanderingHeroes.Engine.Types
{
    public record HexState
    {
        public Hex Hex { get; init; }
        public ImmutableList<Entity> Entities { get; init; }

        public HexState(Hex hex, IEnumerable<Entity> entities)
        {
            Hex = hex;
            Entities = entities.ToImmutableList();
        }
    }
    /// <summary>
    /// Should hold a snapshot of the "mutable" game parts - things that can change each turn.
    /// </summary>
    public record GameState
    {
        public ImmutableDictionary<Hex, HexState> HexStates { get; init; }
        // TODO: this duplication and bad immutability needs rework
        internal Dictionary<int, Entity> _entitiesById;
        private Dictionary<Hex, ImmutableList<int>> _entityIdsForHex;
        public IEnumerable<Entity> Entities => HexStates.Values.SelectMany(hs => hs.Entities);

        public Option<Entity> this[int entityId]
        {
            get => _entitiesById.Lookup(entityId);
        }

        public GameState(IEnumerable<Entity> entities)
        {
            HexStates = entities.GroupBy(e => e.Hex).Select(g => new HexState(g.Key, g)).ToImmutableDictionary(hs => hs.Hex, hs => hs);
            _entityIdsForHex = entities.GroupBy(e => e.Hex).ToDictionary(g => g.Key, g => g.Select(e => e.Id).ToImmutableList());
            _entitiesById = entities.ToDictionary(e => e.Id, e => e);
        }

        public IEnumerable<Entity> EntitiesInRange(Entity ofEntity)
            => ofEntity.Hex.Neighbours()
                .Bind(HexStates.Lookup)
                .SelectMany(hexState => hexState.Entities);

        public GameState AddEntity(Entity entity)
        {
            return this with
            {
                HexStates = HexStates.Lookup(entity.Hex).Match(
                    None: () => HexStates.SetItem(entity.Hex, new HexState(entity.Hex, [entity])),
                    Some: (hs) => HexStates.SetItem(entity.Hex, hs with { Entities = hs.Entities.Add(entity) })
                )
            };
        }
        public GameState ModifyEntities(IEnumerable<Entity> entities)
        {
            var newHexStates = entities.GroupBy(e => e.Hex).Select(g => new HexState(g.Key, g));

            var updatedHexStates = newHexStates.Aggregate<HexState, ImmutableDictionary<Hex, HexState>>(
                seed: HexStates,
                func: (hexStates, updatedHS) => hexStates.Lookup(updatedHS.Hex).Match(
                    None: () => HexStates.Add(updatedHS.Hex, updatedHS),
                    Some: oldHexState => hexStates
                        .Remove(updatedHS.Hex)
                        .Add(updatedHS.Hex, oldHexState with {
                            Entities = oldHexState.Entities
                                .RemoveRange(updatedHS.Entities) //TODO: check that this removes entities matching id
                                .AddRange(updatedHS.Entities) 
                        })
                )
            );

            return this with { HexStates = updatedHexStates };
        }
    }
    public class Game
    {
        public Grid HexMap { get; init; }
        public Transforms Transforms { get; init; }
        public IEnumerable<Entity> Entities => _gameState.Entities;
        private EntityFactory _entityFactory;
        private ImmutableList<IComponent> _components;
        private GameState _gameState;
        public Game(Grid hexMap, Transforms transforms) : this(hexMap, transforms, []) { }
        public Game(Grid hexMap, Transforms transforms, IEnumerable<Entity> entities)
        {
            HexMap = hexMap;
            Transforms = transforms;
            _gameState = new GameState(entities.ToImmutableList());
            _entityFactory = new EntityFactory(entities.Select(e => e.Id).Append(0).Max());
            _components = [new UtilityAIComponent()];
        }
        private T CreateEntityAndAppendToEntities<T>(Func<T> entityCreator) where T : Entity
        {
            var newEntity = entityCreator();

            // This will be potential bad when Entities get bigger, or there's lots to add in a row
            _gameState = _gameState.AddEntity(newEntity);

            return newEntity;
        }
        public SmartEntity CreateSmartEntity(FractionalHex hexCoords, float speed)
            => CreateEntityAndAppendToEntities(()
                => _entityFactory.CreateSmartEntity(hexCoords, speed));
        public Advertiser CreateAdvertiser(FractionalHex hexCoords, IEnumerable<Behaviour> offers)
            => CreateEntityAndAppendToEntities(()
                => _entityFactory.CreateAdvertiser(hexCoords, offers));

        public void Update()
        {
            // run each component, updating the state as we go
            _gameState = _components.Aggregate(
                seed: _gameState,
                func: (state, component) => component.Update(this, state)
            );
        }
        /// <summary>
        /// Expensive call - copies GameState in memory
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SmartEntity AddBehaviour(SmartEntity entity, Behaviour behaviour)
        {
            var updatedAgent = _gameState[entity.Id]
                .Bind(entity => entity is SmartEntity agent ? Some(agent) : None)
                .Match(
                    None: () => entity,
                    Some: agent => agent
                ).Pipe(agent => agent with { Behaviours = agent.Behaviours.Add(behaviour) });

                _gameState = _gameState with
                {
                    HexStates = _gameState.HexStates.Replace(smartEntity, newEntity)
                };
                return newEntity;
            }
            else
            {
                
            }
        }

        // Game related extensions
        public static partial class Extensions
        {

        }
    }
}

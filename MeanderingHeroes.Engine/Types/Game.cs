using MeanderingHeroes.Engine.Components;
using LaYumba.Functional;

namespace MeanderingHeroes.Engine.Types
{
    public record GameState(ImmutableList<Entity> Entities);
    public class Game
    {
        public Grid HexMap { get; init; }
        public Transforms Transforms { get; init; }
        public ImmutableList<Entity> Entities => _gameState.Entities;
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
            // TODO: Partition Entities by Hexes, to use less memory 
            _gameState = _gameState with { Entities = _gameState.Entities.Add(newEntity) };
            return newEntity;
        }
        public SmartEntity CreateSmartEntity(FractionalHex hexCoords, float speed)
            => CreateEntityAndAppendToEntities(()
                => _entityFactory.CreateSmartEntity(hexCoords, speed));
        public Advertiser CreateAdvertiser(FractionalHex hexCoords, IEnumerable<InteractionBase> offers)
            => CreateEntityAndAppendToEntities(()
                => _entityFactory.CreateAdvertiser(hexCoords, offers));

        public void Update()
        {
            // run each component, updating the state as we go
            _gameState = _components.Aggregate(
                seed: _gameState,
                func: (state, component) => component.Update2(this, state)
            );
        }

        public SmartEntity AddBehaviour(SmartEntity entity, Behaviour behaviour)
        {
            var oldEntity = _gameState.Entities.Find(e => e.Id == entity.Id);
            if (oldEntity != null && oldEntity is SmartEntity smartEntity)
            {
                var newEntity = smartEntity with
                {
                    Behaviours = smartEntity.Behaviours.Add(behaviour)
                };
                _gameState = _gameState with
                {
                    Entities = _gameState.Entities.Replace(smartEntity, newEntity)
                };
                return newEntity;
            }
            else
            {
                throw new Exception($"SmartEntity with Id {entity.Id} not found");
            }
        }

        // Game related extensions
        public static partial class Extensions
        {

        }
    }
}

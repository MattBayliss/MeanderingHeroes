using MeanderingHeroes.Engine.Components;
using LaYumba.Functional;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MeanderingHeroes.Engine.Types
{
    public class Game
    {
        public readonly ILoggerFactory LoggerFactory;
        private readonly ILogger Logger;
        public Grid HexMap { get; init; }
        public Transforms Transforms { get; init; }
        public IEnumerable<Entity> Entities => _gameState.Entities;
        private EntityFactory _entityFactory;
        private UtilityAIComponent _utilityAI;
        private GameState _gameState;
        private ConsiderationContext _considerationContext;
        public Game(ILoggerFactory? loggerFactory, Grid hexMap, Transforms transforms) : this(loggerFactory, hexMap, transforms, []) { }
        public Game(ILoggerFactory? loggerFactory, Grid hexMap, Transforms transforms, IEnumerable<Entity> entities)
        {
            LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            Logger = LoggerFactory.CreateLogger<Game>();

            HexMap = hexMap;
            Transforms = transforms;
            _gameState = new GameState(entities);
            _entityFactory = new EntityFactory(entities.Select(e => e.Id).Append(0).Max());
            _considerationContext = new ConsiderationContext();
            _utilityAI = new UtilityAIComponent(LoggerFactory.CreateLogger<UtilityAIComponent>(), _considerationContext);
        }
        public Option<Entity> this[int entityId] => _gameState[entityId];
        private int CreateEntityAndAppendToEntities(Func<Entity> entityCreator)
        {
            var newEntity = entityCreator();

            // This will be potential bad when Entities get bigger, or there's lots to add in a row
            _gameState = _gameState.AddEntity(newEntity);

            return newEntity.Id;
        }
        public int CreateEntity(FractionalHex hexCoords, float speed)
            => CreateEntityAndAppendToEntities(()
                => _entityFactory.CreateEntity(hexCoords, speed));

        public int AddBehaviour(int entityId, BehaviourTemplate behaviourTemplate)
            => _gameState[entityId].Match(
                None: () => 0,
                Some: pawn =>
                {
                    var behaviour = behaviourTemplate(this, pawn);
                    _gameState = _gameState.AddBehaviour(entityId, behaviour);
                    return behaviour.Dse.Id;
                });
        
        public void RemoveBehaviour(int behaviourId)
        {
            _gameState = _gameState.RemoveBehaviours([behaviourId]);
        }

        public List<Dse> GetBehavioursForEntity(int entityId)
            => _gameState
                .Behaviours
                .Where(b => b.EntityId == entityId)
                .Select(b => b.DseId)
                .Bind(_gameState.DseById.Lookup)
                .ToList();

        public void Update()
        {
            // run each component, updating the state as we go
            _gameState = _utilityAI.Update(this, _gameState);
        }
    }
}

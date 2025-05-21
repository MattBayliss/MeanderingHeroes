using MeanderingHeroes.Engine.Components;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MeanderingHeroes.Engine.Types
{
    public class Game
    {
        private readonly ILoggerFactory _loggerFactory;
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
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            Logger = _loggerFactory.CreateLogger<Game>();

            HexMap = hexMap;
            Transforms = transforms;
            _gameState = new GameState(entities);
            _entityFactory = new EntityFactory(entities.Select(e => e.Id).Append(0).Max());
            _considerationContext = new ConsiderationContext();
            _utilityAI = new UtilityAIComponent(_loggerFactory.CreateLogger<UtilityAIComponent>(), _considerationContext);
        }
        public Option<Entity> this[int entityId] => _gameState[entityId];
        private Entity CreateEntityAndAppendToEntities(Func<Entity> entityCreator)
        {
            var newEntity = entityCreator();

            // This will be potential bad when Entities get bigger, or there's lots to add in a row
            _gameState = _gameState.AddEntity(newEntity);

            return newEntity;
        }
        public Entity CreateEntity(FractionalHex hexCoords, float speed)
            => CreateEntityAndAppendToEntities(()
                => _entityFactory.CreateEntity(hexCoords, speed));

        public void AddBehaviour(Entity pawn, BehaviourTemplate behaviour)
        {
            _gameState = _gameState.AddBehaviour(pawn, behaviour(this, pawn));
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

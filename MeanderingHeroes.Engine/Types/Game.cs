using MeanderingHeroes.Engine.Components;
using LaYumba.Functional;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MeanderingHeroes.Engine.Types.Behaviours;

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
        public GameState GameState => _gameState;
        private ConsiderationContext _considerationContext;
        private ImmutableList<Func<Entity, Behaviour>> _baseEntityBehaviourTemplates;
        public Blackboard Blackboard { get; init; }
        public Game(ILoggerFactory? loggerFactory, Grid hexMap, Transforms transforms) : this(loggerFactory, hexMap, transforms, []) { }
        public Game(ILoggerFactory? loggerFactory, Grid hexMap, Transforms transforms, IEnumerable<Entity> entities)
        {
            LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            Logger = LoggerFactory.CreateLogger<Game>();

            Blackboard = new Blackboard();

            HexMap = hexMap;
            Transforms = transforms;
            _gameState = new GameState(entities);
            _entityFactory = new EntityFactory(entities.Select(e => e.Id).Append(0).Max());
            _considerationContext = new ConsiderationContext(this);
            _utilityAI = new UtilityAIComponent(LoggerFactory.CreateLogger<UtilityAIComponent>(), _considerationContext);

            _baseEntityBehaviourTemplates = [
                BehavioursLibrary.MoveToForageFood(this),
                BehavioursLibrary.EatFood(this),
                BehavioursLibrary.GatherFood(this)
            ];
        }
        public Option<Entity> this[int entityId] => _gameState[entityId];
        public void SetFoodItems(IEnumerable<FoodItem> foodItems)
        {
            _gameState = _gameState with { FoodItems = foodItems.ToImmutableHashSet() };
        }
        private int CreateEntityAndAppendToEntities(Func<Entity> entityCreator)
        {
            var newEntity = entityCreator();

            _gameState = _baseEntityBehaviourTemplates.Aggregate(
                seed: _gameState.AddEntity(newEntity),
                func: (state, beFunc) => state.AddBehaviour(newEntity.Id, beFunc(newEntity)));

            return newEntity.Id;
        }
        public int CreateEntity(FractionalHex hexCoords, float speed) => CreateEntity(hexCoords, speed, e => e);
        public int CreateEntity(FractionalHex hexCoords, float speed, Func<Entity, Entity> initFunc)
            => CreateEntityAndAppendToEntities(()
                => _entityFactory.CreateEntity(hexCoords, speed).Pipe(initFunc));

        public int AddBehaviour(int entityId, BehaviourTemplate behaviourTemplate)
            => _gameState[entityId].Match(
                None: () => 0,
                Some: pawn =>
                {
                    var behaviour = behaviourTemplate(this)(pawn);
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
            _considerationContext.SetStateSnapshot();
            // run each component, updating the state as we go
            _gameState = _utilityAI.Update(this, _gameState);
            _gameState = TheMarchOfTime.Update(_gameState);
        }
    }
}

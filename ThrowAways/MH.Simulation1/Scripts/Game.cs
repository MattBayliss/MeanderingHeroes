using Godot;
using MeanderingHeroes.Engine;
using MeanderingHeroes.Engine.Components;
using MeanderingHeroes.Engine.Types;
using MH.Simulation1.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

public partial class Game : Node2D
{
    [Export]
    public float HeroSpeed = 50f;

    private Polygon2D _hero;
    private Dictionary<Hex, HexData> _hexData;
    private TileMapLayer _hexMap;
    private Hex _destination;
    private UtilityAIComponent _utilityAI;

    private static Terrain _grass = new LandTerrain("grass", 1f);
    private static Terrain _forest = new LandTerrain("forest", 3f);
    private static Terrain _hills = new LandTerrain("hills", 2f);
    private static Terrain _sea = new WaterTerrain("sea", 50f);
    private Entity _heroEntity;
    private GameState _gameState;

    public override void _Ready()
    {
        Godot.Engine.TimeScale = 5.0;
        _hero = GetNode<Polygon2D>("Hero");
        _hexMap = GetNode<TileMapLayer>("%HexMap");
        var shortestDistanceBetweenHexes = _hexMap.TileSet.TileSize.Y;
        Functions.UnitsPerHex = shortestDistanceBetweenHexes;
        var startingPos = _hexMap.MapToLocal(new Vector2I(3, 2));
        _hero.Position = startingPos;
        _hexData = LoadMapData(_hexMap);
        Print($"Hexes loaded: {_hexData.Count}");

        _utilityAI = new UtilityAIComponent(_hexData.ToGrid());
        _heroEntity = new Entity(new Point(startingPos.X, startingPos.Y), HeroSpeed);
        _gameState = new GameState([_heroEntity]);
    }

    public void Update()
    {
        var updatedEntity = _utilityAI.Update(_gameState, _heroEntity);
        _gameState = _gameState with { Entities = _gameState.Entities.Replace(_heroEntity, updatedEntity) };
        _heroEntity = updatedEntity;
        var newPosition = new Vector2(_heroEntity.Location.X, _heroEntity.Location.Y);
        _hero.Position = newPosition;
        Print($"{_heroEntity.Location}, {_hero.Position}");
    }

    public override void _Input(InputEvent @event)
    {
        Action handler = @event switch
        {
            InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true, Position: var pos }
                => () => SetDestination(_hexMap.HexForViewport(pos)),
            _ => () => { }
        };
        handler();
    }

    private void SetDestination(Vector2I tileCoords)
    {
        var hex = new Hex(tileCoords.X, tileCoords.Y);
        if(!_hexData.ContainsKey(hex))
        {
            return;
        }

        _destination = hex;

        var moveConsideration = PathFinding.GeneratePathGoalConsideration(
            _utilityAI.Grid, 
            _heroEntity.Location.ToHex(), 
            _destination);

        var updatedEntity = _heroEntity with { Considerations = [moveConsideration] };

        _gameState = _gameState with { Entities = [updatedEntity] };

        _heroEntity = updatedEntity;

        Print($"Destination: {_destination} DestCentre:{_destination.Centre()}, DestHex: {_destination.Centre().ToHex()}, _heroEntity: {_heroEntity.Location}, Hex: {_heroEntity.Location.ToHex()}");
    }

    private Dictionary<Hex, HexData> LoadMapData(TileMapLayer hexMap) =>
        hexMap
            .GetUsedCells()
            .Select(v => (Hex: new Hex(v.X, v.Y), Offset: hexMap.MapToLocal(v), AtlasCoords: hexMap.GetCellAtlasCoords(v)))
            .Select(htd => new HexData(
                Hex: htd.Hex,
                Offset: htd.Offset,
                Terrain: htd.AtlasCoords switch
                {
                    { X: 0, Y: 0 } => _grass,
                    { X: 1, Y: 0 } => _forest,
                    { X: 2, Y: 0 } => _hills,
                    { X: 3, Y: 0 } => _sea,
                    _ => _grass
                })
            ).ToDictionary(hd => hd.Hex, hd => hd);
}
public static partial class Extensions
{
    public static Vector2I HexForViewport(this TileMapLayer tileMapLayer, Vector2 viewportCoords)
        => tileMapLayer.LocalToMap(tileMapLayer.MakeCanvasPositionLocal(viewportCoords));

    public static Grid ToGrid(this Dictionary<Hex, HexData> hexData)
        => new Grid(hexData.Select(kvp => (kvp.Key, kvp.Value.Terrain)));
}
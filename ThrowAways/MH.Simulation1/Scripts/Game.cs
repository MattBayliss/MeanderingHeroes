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
    private Transform2D _toHexSpaceMatrix;
    private Transform2D _toGameSpaceMatrix;

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

        (_toHexSpaceMatrix, _toGameSpaceMatrix) = GetTransformMatrices(_hexMap);

        var startingPos = _hexMap.MapToLocal(new Vector2I(0, 0));
        _hero.Position = startingPos;
        _hexData = LoadMapData(_hexMap);
        Print($"Hexes loaded: {_hexData.Count}");

        _utilityAI = new UtilityAIComponent(_hexData.ToGrid());
        _heroEntity = new Entity((_toHexSpaceMatrix * startingPos).ToPoint(), HeroSpeed);
        _gameState = new GameState([_heroEntity]);
    }

    public void Update()
    {
        var updatedEntity = _utilityAI.Update(_gameState, _heroEntity);
        _gameState = _gameState with { Entities = _gameState.Entities.Replace(_heroEntity, updatedEntity) };
        _heroEntity = updatedEntity;
        var newPosition = _toGameSpaceMatrix * _heroEntity.Location.ToVector2();
        _hero.Position = newPosition;
    }

    public override void _Input(InputEvent @event)
    {
        Action handler = @event switch
        {
            InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true, Position: var pos }
                => () => SetDestination(pos),
            _ => () => { }
        };
        handler();
    }

    private void SetDestination(Vector2 viewPortPosition)
    {
        var position = _hexMap.MakeCanvasPositionLocal(viewPortPosition);
        var tileCoords = _hexMap.LocalToMap(position);
        Print($"tile clicked: {tileCoords}");
        var hex = new Hex(tileCoords.X, tileCoords.Y);
        if (!_hexData.ContainsKey(hex))
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

        // TODO: delete this destination debugging
        var destHexPoint = _destination.Centre();
        var destHexVector2 = _toGameSpaceMatrix * destHexPoint.ToVector2();
        var destTileVector = _hexMap.MapToLocal(new Vector2I(_destination.Q, _destination.R));
        var tileForVector2 = _hexMap.LocalToMap(destHexVector2);

        // clicking on as close to centre of title 0,0 as possible
        // VPPos: (166, 60), Positon: (14.285706, 16.027866), tileCoords: (0, 0), Destination: Hex { Q = 0, R = 0, S = 0 } destPoint:Point { X = 0, Y = 0 }|(0, 0), destVector2: (-14.5, -16.5), tileForVector2: (0, -2), heroPoint: Point { X = 0, Y = 0 }, heroVector2: (-14.5, -16.5), heroHex: $Hex { Q = 0, R = 0, S = 0 }

        Print($"VPPos: {viewPortPosition}, Positon: {position}, tileCoords: {tileCoords}, Destination: {_destination} destHexPoint:{destHexPoint}, destHexVector2: {destHexVector2}, destTileVector: {destTileVector}, heroPoint: {_heroEntity.Location}, heroVector2: {_toGameSpaceMatrix * _heroEntity.Location.ToVector2()}, heroHex: {_heroEntity.Location.ToHex()}");
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
    private static (Transform2D ToHexSpace, Transform2D ToGameSpace) GetTransformMatrices(TileMapLayer hexMap)
    {
        var offset = hexMap.MapToLocal(Vector2I.Zero);
        Print($"Offset: {offset}");

        var refX = new Vector2(hexMap.TileSet.TileSize.X, 0);
        var refY = new Vector2(0, hexMap.TileSet.TileSize.Y) 
            * (Mathf.Sqrt(3) / 2f); // finding hex `size` from height - to calculate Y axis adjustment

        Print($"Offset: {offset}, refX:{refX}, refY:{refY}");

        var tileSize = hexMap.TileSet.TileSize;
        var toGameSpace = new Transform2D(
            xAxis: refX,
            yAxis: refY,
            originPos: offset
        );

        return (toGameSpace.AffineInverse(), toGameSpace);
    }
}
public static partial class Extensions
{
    public static Vector2I HexForViewport(this TileMapLayer tileMapLayer, Vector2 viewportCoords)
        => tileMapLayer.LocalToMap(tileMapLayer.MakeCanvasPositionLocal(viewportCoords));

    public static Grid ToGrid(this Dictionary<Hex, HexData> hexData)
        => new Grid(hexData.Select(kvp => (kvp.Key, kvp.Value.Terrain)));

    public static Point ToPoint(this Vector2 vector2) => new Point(vector2.X, vector2.Y);
    public static Vector2 ToVector2(this Point point) => new Vector2(point.X, point.Y);
}
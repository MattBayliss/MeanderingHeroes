using Godot;
using MeanderingHeroes.Engine;
using MeanderingHeroes.Engine.Components;
using MeanderingHeroes.Engine.Types;
using MH.Simulation1.Types;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public override void _Ready()
    {
        Godot.Engine.TimeScale = 5.0;
        _hero = GetNode<Polygon2D>("Hero");
        _hexMap = GetNode<TileMapLayer>("%HexMap");

        var tileSize = _hexMap.TileSet.TileSize;
        Transforms.Init(_hexMap.MapToLocal(new Vector2I(0, 0)).ToDotNetVector2(), tileSize.X, tileSize.Y);

        var startingPos = _hexMap.MapToLocal(new Vector2I(0, 0));
        _hero.Position = startingPos;
        _hexData = LoadMapData(_hexMap);
        Print($"Hexes loaded: {_hexData.Count}");

        _utilityAI = new UtilityAIComponent(_hexData.ToGrid());
        _heroEntity = new Entity(new FractionalHex(0f, 0f), HeroSpeed);
    }

    public void Update()
    {
        var updatedEntity = _utilityAI.Update(_heroEntity);
        _heroEntity = updatedEntity;
        _hero.Position = updatedEntity.Location.ToGodotVector();
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
            _heroEntity.HexCoords.Round(),
            _destination);

        var updatedEntity = _heroEntity with { Considerations = [moveConsideration] };

        _heroEntity = updatedEntity;
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

    public static System.Numerics.Vector2 ToDotNetVector2(this Godot.Vector2 vector2) => new System.Numerics.Vector2(vector2.X, vector2.Y);
    public static Godot.Vector2 ToGodotVector(this System.Numerics.Vector2 vector2) => new Godot.Vector2(vector2.X, vector2.Y);
}
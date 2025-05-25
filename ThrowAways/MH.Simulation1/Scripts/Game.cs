using Godot;
using LaYumba.Functional;
using MeanderingHeroes.Engine;
using MeanderingHeroes.Engine.Components;
using MeanderingHeroes.Engine.Types;
using MH.Simulation1.Types;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using static LaYumba.Functional.F;

public partial class Game : Node2D
{
    [Export]
    public float HeroSpeed = 50f;

    private Polygon2D _hero;
    private Dictionary<Hex, HexData> _hexData;
    private TileMapLayer _hexMap;
    private Hex _destination;
    private UtilityAIComponent _utilityAI;
    private MeanderingHeroes.Engine.Types.Game _gameEngine;

    private static Terrain _grass = new LandTerrain("grass", 1f);
    private static Terrain _forest = new LandTerrain("forest", 3f);
    private static Terrain _hills = new LandTerrain("hills", 2f);
    private static Terrain _sea = new WaterTerrain("sea", 50f);
    private int _heroId;
    private int _runningDSEId = 0;

    public override void _Ready()
    {
        Godot.Engine.TimeScale = 5.0;
        _hero = GetNode<Polygon2D>("Hero");
        _hexMap = GetNode<TileMapLayer>("%HexMap");

        var tileSize = _hexMap.TileSet.TileSize;
        
        var startingPos = _hexMap.MapToLocal(new Vector2I(0, 0));

        _hero.Position = startingPos;

        _hexData = LoadMapData(_hexMap);
        Print($"Hexes loaded: {_hexData.Count}");

        var hexOffset = startingPos.ToDotNetVector2();

        

        _gameEngine = new MeanderingHeroes.Engine.Types.Game(GodotLogging.GodotLoggerFactory(), _hexData.ToGrid(), new Transforms(hexOffset, tileSize.X, tileSize.Y));
        _heroId = _gameEngine.CreateEntity(new FractionalHex(0f, 0f), HeroSpeed);
    }

    public void Update()
    {
        _gameEngine.Update();
        var optionEntity = _gameEngine[_heroId];
        optionEntity.ForEach(updated =>
        {
            _hero.Position = _gameEngine.ToGameXY(updated.HexCoords).ToGodotVector();
            Print(_hero.Position.ToString());
        });
        
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
        
        Print($"tile clicked: {tileCoords} - AtlasCoords: {_hexMap.GetCellAtlasCoords(tileCoords)}");

        var hex = new Hex(tileCoords.X, tileCoords.Y);

        if(!_gameEngine.HexMap.InBounds(hex))
        {
            Print($"destination out of bounds: {hex}");
            return;
        }

        if(_runningDSEId > 0)
        {
            _gameEngine.RemoveBehaviour( _runningDSEId );
        }

        _destination = hex;

        _runningDSEId = _gameEngine.AddBehaviour(_heroId, BehavioursLibrary.PlayerSetDestination(_destination));
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

    public static System.Numerics.Vector2 ToDotNetVector2(this Godot.Vector2 vector2) => new System.Numerics.Vector2(vector2.X, vector2.Y);
    public static Godot.Vector2 ToGodotVector(this System.Numerics.Vector2 vector2) => new Godot.Vector2(vector2.X, vector2.Y);
}
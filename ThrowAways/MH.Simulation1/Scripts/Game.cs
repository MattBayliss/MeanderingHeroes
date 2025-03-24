using Godot;
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
    private Hero _hero;
    private Dictionary<Hex, HexData> _hexData;
    private TileMapLayer _hexMap;
    private Hex _destination;
    private UtilityAIComponent _utilityAI;

    private static Terrain _grass = new LandTerrain("grass", 1f);
    private static Terrain _forest = new LandTerrain("forest", 3f);
    private static Terrain _hills = new LandTerrain("hills", 2f);
    private static Terrain _sea = new WaterTerrain("sea", 50f);

    public override void _Ready()
    {
        _hero = GetNode<Hero>("Hero");
        _hexMap = GetNode<TileMapLayer>("%HexMap");
        _hero.Position = _hexMap.MapToLocal(new Vector2I(3, 2));
        _hexData = LoadMapData(_hexMap);
        Print($"Hexes loaded: {_hexData.Count}");

        _utilityAI = new UtilityAIComponent(_hexData.ToGrid());
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

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    private void SetDestination(Vector2I tileCoords)
    {
        var hex = new Hex(tileCoords.X, tileCoords.Y);
        if(!_hexData.ContainsKey(hex))
        {
            return;
        }

        _destination = hex;

        Print($"Destination: {_destination}");
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
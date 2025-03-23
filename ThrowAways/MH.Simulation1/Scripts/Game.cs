using Godot;
using MeanderingHeroes.Engine.Types;
using MH.Simulation1.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class Game : Node2D
{
    private Hero _hero;
    private Dictionary<Hex, HexData> _hexData;

    public override void _Ready()
    {
        _hero = GetNode<Hero>("Hero");
        var hexMap = GetNode<TileMapLayer>("%HexMap");
        _hero.Position = hexMap.MapToLocal(new Vector2I(3, 2));
        _hexData = LoadMapData(hexMap);
        Print($"Hexes loaded: {_hexData.Count}");
    }

    private Dictionary<Hex, HexData> LoadMapData(TileMapLayer hexMap) =>
        hexMap
            .GetUsedCells()
            .Select(v => (Hex: new Hex(v.X, v.Y), Offset: hexMap.MapToLocal(v), AtlasCoords: hexMap.GetCellAtlasCoords(v)))
            .Select(htd => new HexData(
                Hex: htd.Hex,
                Offset: htd.Offset,
                MovementCost: htd.AtlasCoords switch
                {
                    { X: 0, Y: 0 } => 1f, // grass
                    { X: 1, Y: 0 } => 3f, // forest
                    { X: 2, Y: 0 } => 2f, // hills
                    { X: 3, Y: 0 } => 10f, // water
                    _ => 1.0f
                })
            ).ToDictionary(hd => hd.Hex, hd => hd);
}

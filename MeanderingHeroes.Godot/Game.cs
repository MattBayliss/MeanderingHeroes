using Godot;
using MeanderingHeroes.Engine.Types;
using System;

namespace MeanderingHeroes.Godot
{
    public partial class Game : Node2D
    {
        public override void _Ready()
        {
            var hex = new Hex(1, 2);
            GD.Print($"hex: {hex}");
        }
    }
}

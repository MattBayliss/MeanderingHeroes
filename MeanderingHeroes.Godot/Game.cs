using Godot;
using MeanderingHeroes.Engine.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static Godot.GD;

namespace MeanderingHeroes.Godot
{
    public partial class Game : Node2D
    {
        [Export]
        public float HeroSpeed { get; set; } = 50f;
        private TileMapLayer _tileMap;
        private Vector2? _destination;
        private Node2D _hero;
        public override void _Input(InputEvent @event)
        {
            switch (@event)
            {
                case InputEventMouseButton { Pressed: true, Position: var pos }:
                    var aniSprite = _hero.GetChild<AnimatedSprite2D>(0);
                    aniSprite.Play("walk");
                    _destination = _tileMap.MapToLocal(_tileMap.LocalToMap(pos));
                    Print($"clicked: {_tileMap.LocalToMap(pos).ToHex()}");
                    break;
            }
        }
        public override void _PhysicsProcess(double delta)
        {
            if(_destination == null) return;


            var heroPos = _hero.Position;
            if(heroPos == _destination)
            {
                _destination = null;
                return;
            }

            var vector = (Vector2)_destination - heroPos;
            vector = vector.LimitLength(HeroSpeed * (float)delta);

            _hero.Position = heroPos + vector;
        }
        [MemberNotNull(nameof(_hero), nameof(_tileMap))]
        public override void _Ready()
        {
            _hero = GetNode<Node2D>("Hero");

            _tileMap = GetNode<TileMapLayer>("TileMapLayer");

            var hex = new Hex(1, 2);
            Print($"hex: {hex}");
        }
    }
}

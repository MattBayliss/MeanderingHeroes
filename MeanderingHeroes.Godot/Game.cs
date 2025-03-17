using Godot;
using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static Godot.GD;
using static LaYumba.Functional.F;
using static MeanderingHeroes.Godot.GodotObjectHelpers;

namespace MeanderingHeroes.Godot
{
    public partial class Game : Node2D
    {
        [Export]
        public float HeroSpeed { get; set; } = 50f;

        private Option<TileMapLayer> _tileMap;
        private Option<Hero> _hero;

        public override void _Ready()
        {
            _hero = GetNode<Hero>("Hero").ToOption();
            _tileMap = GetNode<TileMapLayer>("TileMapLayer").ToOption();

        }
        public override void _Input(InputEvent @event)
        {
            _hero.Bind(hero => _tileMap.Map(tm => @event switch
                    {
                        InputEventMouseButton { Pressed: true, Position: var pos }
                            => SetDestinationForHero(hero, pos),
                        _ => () => { }
                    })
            ).Do(handler => handler());
        }

        private Action SetDestinationForHero(Hero hero, Vector2 destination) => () => hero.SetDestination(destination);
    }
}


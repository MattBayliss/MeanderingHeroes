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
                        InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true, Position: var pos }
                            => SetDestinationForHero(hero, PositionToHexCentre(tm, pos)),
                        InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true, Position: var pos }
                            => () => AttemptTileWrite(tm, pos),
                        _ => () => { }
                    })
            ).Do(handler => handler());
        }
        private static Vector2 PositionToHexCentre(TileMapLayer tileMap, Vector2 position) 
            => tileMap.MapToLocal(tileMap.LocalToMap(position));

        private void AttemptTileWrite(TileMapLayer tileMap, Vector2 position)
        {
            Print(tileMap.LocalToMap(position));
            tileMap.SetCell(tileMap.LocalToMap(position), 2, new Vector2I(0, 1));
        }

        private Action SetDestinationForHero(Hero hero, Vector2 destination) => () => hero.SetDestination(destination);
    }
}


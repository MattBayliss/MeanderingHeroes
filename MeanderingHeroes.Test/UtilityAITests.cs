using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using System.Numerics;
using Xunit.Abstractions;

namespace MeanderingHeroes.Test
{
    public class UtilityAITests(ITestOutputHelper output)
    {
        [Fact]
        public void DetoursForFood()
        {
            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(Vector2.Zero, 1f, 2f / MathF.Sqrt(3)),
                entities: []
            );

            // slow hero, with Hunger set
            Utility startingHunger = 0.6f;
            var heroId = game.CreateEntity((1.0f, 6.0f), 0.1f, entity => entity with { Hunger = startingHunger });

            Hex destination = (6, 6);
            // un-detoured path should be (1,6), (2,6), (3,6), (4,6), (5,6), (6,6)
            game.AddBehaviour(heroId, BehavioursLibrary.PlayerSetDestination(destination));

            FractionalHex foodCoords = (3, 4);
            var foodItem = new FoodItem(foodCoords, FoodType.Berry, 1f);
            game.SetFoodItems([foodItem]);

            List<(FractionalHex Coords, Hex Hex, Utility Hunger)> stateSnapshots = [];

            do
            {
                game.Update();

                var hero = Helpers.AssertIsSome<Entity>(game[heroId]);
                stateSnapshots.Add((hero.HexCoords, hero.HexCoords.Round(), hero.Hunger));

            } while (game[heroId].Map(hero => hero.HexCoords).GetOrElse((0, 0)) != destination);

            Assert.True(stateSnapshots.Any());
            Assert.Contains(stateSnapshots, s => s.Hex == foodCoords.Round());
            Assert.Contains(stateSnapshots, s => s.Hunger < startingHunger);
        }
    }
}

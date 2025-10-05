using MeanderingHeroes.Engine.Types;
using MeanderingHeroes.Engine.Types.Considerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace MeanderingHeroes.Test
{
    public class ConsiderationTests(ITestOutputHelper output)
    {
        [Fact]
        public void FoodConsiderationTests()
        {

            List<StateChange> changes = [];

            var stateChangeTestComponent = Helpers.StateChangeEventTestComponent(() => changes);

            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(Vector2.Zero, 1f, 2f / MathF.Sqrt(3)),
                components: [stateChangeTestComponent],
                entities: []
            );


            // food supply is between 0 and 5.0
            float foodSupply = 4f;
            Utility expectedFoodUtility = foodSupply / 5f;

            var foodItem = new LayerItem((3f, 1f), LayerItemType.Food, (int)FoodType.Berry, 1f);

            game.SetFoodItems([foodItem]);

            var considerationContext = new ConsiderationContext(game);

            Utility hunger = 0.3f;
            var heroId = game.CreateEntity((1.0f, 1.0f), 0.1f, entity => entity with { FoodSupply = foodSupply, Hunger = hunger });
            var hero = Helpers.AssertIsSome<Entity>(game[heroId]);

            // give the hero knowledge of the food (otherwise the hero would have to search for it)
            var layerItemFound = StateChange.LayerItemFound(heroId, foodItem);
            var foodTidbit = StateChange.TidbitLearnt(heroId, ConsiderationType.HexFood, foodItem.HexCoords.Round(), foodItem.Quality);

            changes = [foodTidbit, layerItemFound];

            game.Update();

            var distanceToFood = hero.HexCoords.Distance(foodItem.HexCoords);

            Assert.True(distanceToFood < 10f);

            Assert.Equal((Utility)0f, ConsiderationContext.DistanceToHex(hero.HexCoords)(hero));

            Assert.Equal(expectedFoodUtility, ConsiderationContext.FoodSupply(hero));
            Assert.Equal(hunger, ConsiderationContext.PawnHunger(hero));
            var foodDistance = Helpers.AssertIsSome<Utility>(considerationContext.ForageFoodDistance.Get(hero));
            Assert.True(foodDistance.Value > 0f);

            Assert.True(foodDistance.Value < 1f);

        }
    }
}

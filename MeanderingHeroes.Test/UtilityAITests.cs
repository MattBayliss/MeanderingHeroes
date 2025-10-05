using LaYumba.Functional;
using MeanderingHeroes.Engine.Types;
using MeanderingHeroes.Engine.Types.Behaviours;
using System.Numerics;
using Xunit.Abstractions;

namespace MeanderingHeroes.Test
{
    public class UtilityAITests(ITestOutputHelper output)
    {
        [Fact]
        public void GatherAndEatFoodAtYourFeet()
        {
            List<StateChange> changes = [];

            var stateChangeTestComponent = Helpers.StateChangeEventTestComponent(() => changes);

            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(Vector2.Zero, 1f, 2f / MathF.Sqrt(3)),
                components: [stateChangeTestComponent, Game.UtilityAIComponent],
                entities: []
            );

            // slow hero, with Hunger set
            Utility startingHunger = 0.9f;
            float foodSupply = 0f;
            var heroId = game.CreateEntity((3f, 3f), 0.1f, entity => entity with { Hunger = startingHunger, FoodSupply = foodSupply });

            FractionalHex foodCoords = (3, 3);
            var foodItem = new LayerItem(foodCoords, LayerItemType.Food, (int)FoodType.Berry, 1f);
            game.SetFoodItems([foodItem]);

            // give the hero knowledge of the food (otherwise the hero would have to search for it)
            var layerItemFound = StateChange.LayerItemFound(heroId, foodItem);
            var foodTidbit = StateChange.TidbitLearnt(heroId, ConsiderationType.HexFood, foodItem.HexCoords.Round(), foodItem.Quality);

            changes = [layerItemFound, foodTidbit];
            game.Update();
            changes = [];

            List<(float FoodSupply, Utility Hunger, float FoodQuality)> stateSnapshots = [];

            for (int i = 0; i < 10; i++)
            {
                game.Update();
                var hero = Helpers.AssertIsSome<Entity>(game[heroId]);
                var food = Helpers.AssertIsSome<LayerItem>(game.GetLayerItem(foodItem.Id));
                stateSnapshots.Add((hero.FoodSupply, hero.Hunger, food.Quality));
            }

            Assert.DoesNotContain(stateSnapshots, ss => ss.FoodSupply < 0);
            Assert.Contains(stateSnapshots, ss => ss.FoodSupply > 0);
            Assert.Contains(stateSnapshots, ss => ss.Hunger < startingHunger);
            Assert.DoesNotContain(stateSnapshots, ss => ss.FoodQuality > foodItem.Quality);

            var statePairs = stateSnapshots.Zip(stateSnapshots.Skip(1));

            // there should be an instance where the food supply goes up via gathering
            Assert.Contains(statePairs, tuple => tuple.Second.FoodSupply > tuple.First.FoodSupply);

            // there should be an instance where food quality goes down because of eating
            Assert.Contains(statePairs, tuple => tuple.Second.FoodQuality < tuple.First.FoodQuality);

            // there should be an instance where hunger goes down and food supply goes down
            Assert.Contains(statePairs, tuple => tuple.Second.FoodSupply < tuple.First.FoodSupply && tuple.Second.Hunger < tuple.First.Hunger);
        }
        [Fact]
        public void DetoursForFood()
        {
            List<StateChange> changes = [];

            var stateChangeTestComponent = Helpers.StateChangeEventTestComponent(() =>
            {
                return changes;
            }
            );

            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(Vector2.Zero, 1f, 2f / MathF.Sqrt(3)),
                components: [stateChangeTestComponent, Game.UtilityAIComponent],
                entities: []
            );

            // slow hero, with Hunger set
            Utility startingHunger = 0.9f;
            float foodSupply = 0f;
            var heroId = game.CreateEntity((1.0f, 6.0f), 0.1f, entity => entity with { Hunger = startingHunger, FoodSupply = foodSupply });

            Hex destination = (6, 6);
            // un-detoured path should be (1,6), (2,6), (3,6), (4,6), (5,6), (6,6)
            game.AddBehaviour(heroId, BehavioursLibrary.PlayerSetDestination(destination));

            FractionalHex foodCoords = (3, 4);
            var foodItem = new LayerItem(foodCoords, LayerItemType.Food, (int)FoodType.Berry, 1f);
            game.SetFoodItems([foodItem]);

            // give the hero knowledge of the food (otherwise the hero would have to search for it)
            var layerItemFound = StateChange.LayerItemFound(heroId, foodItem);
            var foodTidbit = StateChange.TidbitLearnt(heroId, ConsiderationType.HexFood, foodItem.HexCoords.Round(), foodItem.Quality);

            changes = [layerItemFound, foodTidbit];
            game.Update();
            changes = [];

            var stateLayerItem = Assert.Single(game.GameState.FoodItems.Items);
            Assert.Equal(foodItem, stateLayerItem);

            List<(FractionalHex Coords, Hex Hex, Utility Hunger)> stateSnapshots = [];

            int attempts = 0;

            do
            {
                game.Update();

                var hero = Helpers.AssertIsSome<Entity>(game[heroId]);
                stateSnapshots.Add((hero.HexCoords, hero.HexCoords.Round(), hero.Hunger));
                attempts++;

            } while (attempts < 200 && game[heroId].Map(hero => hero.HexCoords).GetOrElse((0, 0)) != destination);

            Assert.NotEqual(200, attempts);
            Assert.True(stateSnapshots.Any());
            Assert.Contains(stateSnapshots, s => s.Hex == foodCoords.Round());
            Assert.Contains(stateSnapshots, s => s.Hunger < startingHunger);
        }
        [Fact]
        public void LayerStateChange()
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
            var layerItem = new LayerItem((1.2f, 3.4f), LayerItemType.Food, (int)FoodType.Berry, 4f);
            var different = layerItem with { Quality = 3f };

            Assert.NotEqual(layerItem, different);
            Assert.Equal(layerItem.HexCoords, different.HexCoords);
            Assert.Equal(layerItem.ItemType, different.ItemType);
            Assert.Equal(layerItem.SubType, different.SubType);

            game.SetFoodItems([layerItem]);

            Assert.Single(game.GameState.FoodItems.Items);
            Assert.Equal(layerItem, game.GameState.FoodItems.Items[0]);

            var layerItemFromGame = Helpers.AssertIsSome<LayerItem>(game.GetLayerItem(layerItem.Id));
            Assert.Equal(layerItem, layerItemFromGame);

            changes = [new LayerItemChange(layerItem, different)];
            game.Update();
            layerItemFromGame = Helpers.AssertIsSome<LayerItem>(game.GetLayerItem(layerItem.Id));

            Assert.Equal(different, layerItemFromGame);

            changes = [new LayerItemChange(layerItemFromGame, layerItemFromGame with { Quality = layerItemFromGame.Quality - 0.1f })];

            game.Update();

            var lastLayerItemFromGame = Helpers.AssertIsSome<LayerItem>(game.GetLayerItem(layerItem.Id));

            // make sure these objects are immutable and haven't changed
            Assert.Equal(different, layerItemFromGame);

            Assert.NotEqual(layerItemFromGame, lastLayerItemFromGame);

            Assert.Equal(layerItemFromGame.Quality - 0.1f, lastLayerItemFromGame.Quality);



        }
        [Fact]
        public void MovesToFoodWhenHungerGetsHigh()
        {
            List<StateChange> changes = [];

            var stateChangeTestComponent = Helpers.StateChangeEventTestComponent(() => changes);

            var game = new Game(
                loggerFactory: output.ToLoggerFactory(),
                hexMap: Helpers.MakeGrass10x10MapGrid(),
                transforms: new Transforms(Vector2.Zero, 1f, 2f / MathF.Sqrt(3)),
                components: [stateChangeTestComponent, Game.UtilityAIComponent, Game.MarchOfTimeComponent],
                entities: []
            );

            // slow hero, with Hunger set
            Utility startingHunger = 0.0f;
            float foodSupply = 0f;
            var heroId = game.CreateEntity((1.0f, 6.0f), 0.3f, entity => entity with { Hunger = startingHunger, FoodSupply = foodSupply });

            FractionalHex foodCoords = (3, 4);
            var foodItem = new LayerItem(foodCoords, LayerItemType.Food, (int)FoodType.Berry, 1f);
            game.SetFoodItems([foodItem]);

            // give the hero knowledge of the food (otherwise the hero would have to search for it)
            var layerItemFound = StateChange.LayerItemFound(heroId, foodItem);
            var foodTidbit = StateChange.TidbitLearnt(heroId, ConsiderationType.HexFood, foodItem.HexCoords.Round(), foodItem.Quality);

            changes = [layerItemFound, foodTidbit];
            game.Update();
            changes = [];

            List<(FractionalHex Coords, float FoodSupply, float Hunger)> stateSnapshots = [];

            int attempts = 0;

            float startingSupply = Helpers.AssertIsSome<Entity>(game[heroId]).FoodSupply;

            do
            {
                game.Update();

                var hero = Helpers.AssertIsSome<Entity>(game[heroId]);
                stateSnapshots.Add((hero.HexCoords, hero.FoodSupply, hero.Hunger));
                attempts++;

            } while (attempts < 1000 && !game[heroId].Map(hero => hero.HexCoords).GetOrElse((0, 0)).Equals(foodCoords));

            Assert.NotEqual(1000, attempts);
            // should at least be a gather and an eat
            Assert.True(attempts > 1);

            // hero made it to the food - hunger should have gone up
            Assert.Contains(stateSnapshots, s => s.Hunger > startingHunger);
            // and not satiated yet
            Assert.DoesNotContain(stateSnapshots, s => s.Hunger < startingHunger);
            // food supply should remain unchanged
            Assert.DoesNotContain(stateSnapshots, s => s.FoodSupply != startingSupply);

            float currentHunger = Helpers.AssertIsSome<Entity>(game[heroId]).Hunger;

            stateSnapshots = [];

            attempts = 0;

            // try 1000 more updates - should see some gathering and eating?
            for (int i = 0; i < 1000; i++)
            {

                game.Update();

                var hero = Helpers.AssertIsSome<Entity>(game[heroId]);
                stateSnapshots.Add((hero.HexCoords, hero.FoodSupply, hero.Hunger));
                attempts++;
            }

            Assert.True(stateSnapshots.Any());
            var snapshotPairs = stateSnapshots.Zip(stateSnapshots.Skip(1));
            // hunger has gone down (eating happened)
            Assert.Contains(snapshotPairs, pair => pair.Second.Hunger < pair.First.Hunger);
            Assert.Contains(stateSnapshots, s => s.FoodSupply > 0f);
            Assert.DoesNotContain(stateSnapshots, s => s.Coords != foodCoords);
        }
    }
}


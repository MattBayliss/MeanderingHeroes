using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public enum LayerItemType
    {
        Food,
        Shelter
    }
    public enum FoodType
    {
        Berry,
        Nut,
        Fruit,
        Tuber
    }
    public record LayerItem(FractionalHex HexCoords, LayerItemType ItemType, int SubType, float Quality);
    public record FoodItem(FractionalHex HexCoords, FoodType FoodType, float Quality) : LayerItem(HexCoords, LayerItemType.Food, (int)FoodType, Quality);

    public record Layer<T>(ImmutableHashSet<T> LayerItems) where T:LayerItem;

    public record ForageFoodLayer(ImmutableHashSet<FoodItem> FoodItems) : Layer<FoodItem>(FoodItems);
}

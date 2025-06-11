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
    public record LayerItem
    {
        private static int _lastId = 0;
        public int Id { get; init; }
        public FractionalHex HexCoords { get; init; }
        public LayerItemType ItemType { get; init; }
        public int SubType { get; init; }
        public float Quality { get; init; }
        public LayerItem(FractionalHex hexCoords, LayerItemType itemType, int subType, float quality)
        {
            Id = _lastId++;
            HexCoords = hexCoords;
            ItemType = itemType;
            SubType = subType;
            Quality = quality;
        }
    }
    public record FoodItem(FractionalHex HexCoords, FoodType FoodType, float Quality) : LayerItem(HexCoords, LayerItemType.Food, (int)FoodType, Quality);

    public record Layer<T>(ImmutableHashSet<T> LayerItems) where T:LayerItem;

    public record ForageFoodLayer(ImmutableHashSet<FoodItem> FoodItems) : Layer<FoodItem>(FoodItems);
}

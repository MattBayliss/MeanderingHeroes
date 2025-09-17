using LaYumba.Functional;

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
    public readonly record struct LayerItem
    {
        private static int _lastId = 0;
        public int Id { get; init; }
        public FractionalHex HexCoords { get; init; }
        public LayerItemType ItemType { get; init; }
        public int SubType { get; init; }
        public float Quality { get; init; }
        public LayerItem(FractionalHex hexCoords, LayerItemType itemType, int subType, float quality)
        {
            Id = ++_lastId;
            HexCoords = hexCoords;
            ItemType = itemType;
            SubType = subType;
            Quality = quality;
        }
        public override string ToString() => $"[{ItemType.ToString(), 8}: {Id}|{HexCoords}|{SubType}|{Quality:F2}]";
    }
    public record Layer
    {
        // TODO: duplicate collections - refactor down to the best one once I know which one
        // is more used
        public ImmutableList<LayerItem> Items { get; init; }
        public ImmutableDictionary<Hex, ImmutableList<LayerItem>> ItemsByHex { get; init;}
        public Layer(IEnumerable<LayerItem> layerItems)
        {
            Items = layerItems.ToImmutableList();
            ItemsByHex = layerItems
                .GroupBy(li => li.HexCoords.Round())
                .ToImmutableDictionary(g => g.Key, g => g.ToImmutableList());
        }
        public Layer(IDictionary<Hex, ImmutableList<LayerItem>> layerItems)
        {
            Items = layerItems.Values.Flatten().ToImmutableList();
            ItemsByHex = layerItems.ToImmutableDictionary();
        }
    }
}

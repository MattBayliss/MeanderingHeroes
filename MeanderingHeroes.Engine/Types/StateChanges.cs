using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record StateChange
    {
        public override string ToString() => $"{this.GetType().Name, -16}";
        // TODO: add the rest of the factory methods
        public static StateChange TidbitLearnt(int entityId, ConsiderationType considerationType, Hex hex, Utility value) 
            => new TidbitLearnt(entityId, new(considerationType, hex, 0, value, 0f));
        public static StateChange TidbitLearnt(int entityId, ConsiderationType considerationType, Hex hex, float a, float b) 
            => new TidbitLearnt(entityId, new(considerationType, hex, 0,a, b));
        public static StateChange LayerItemFound(int entityId, LayerItem layerItem)
            => new LayerItemFound(entityId, layerItem);
    }
    public record LayerItemFound(int EntityId, LayerItem LayerItem) : StateChange
    {
        public override string ToString() => $"[{base.ToString()}: EntityId: {EntityId}, LayerItem: {LayerItem.ToString()}]";
    }
    public record LayerItemChange(LayerItem OldItem, LayerItem NewItem) : StateChange
    {
        public override string ToString() => $"[{base.ToString()}: Old: {OldItem.ToString()}, New: {NewItem.ToString()}]";
    }
    public record EntityChange(Entity UpdatedEntity) : StateChange
    {
        public override string ToString() => $"[{base.ToString()}: {UpdatedEntity.ToString()}]";
    }
    public record TidbitLearnt(int EntityId, Tidbit Tidbit) : StateChange
    {
        public override string ToString() => $"[{base.ToString()}: {Tidbit.ToString()}]";
    }
}

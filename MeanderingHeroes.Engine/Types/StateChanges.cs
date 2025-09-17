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
        public static StateChange TidbitLearnt(ConsiderationType considerationType, Hex hex, Utility value) 
            => new TidbitLearnt(new(considerationType, hex, value, 0f));
        public static StateChange TidbitLearnt(ConsiderationType considerationType, Hex hex, float a, float b) 
            => new TidbitLearnt(new(considerationType, hex, a, b));
    }
    public record LayerItemChange(LayerItem OldItem, LayerItem NewItem) : StateChange
    {
        public override string ToString() => $"[{base.ToString()}: Old: {OldItem.ToString()}, New: {NewItem.ToString()}]";
    }
    public record EntityChange(Entity UpdatedEntity) : StateChange
    {
        public override string ToString() => $"[{base.ToString()}: {UpdatedEntity.ToString()}]";
    }
    public record TidbitLearnt(Tidbit Tidbit) : StateChange
    {
        public override string ToString() => $"[{base.ToString()}: {Tidbit.ToString()}]";
    }
}

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
    }
    public record LayerItemChange(LayerItem OldItem, LayerItem NewItem) : StateChange
    {
        public override string ToString() => $"[{base.ToString()}: Old: {OldItem.ToString()}, New: {NewItem.ToString()}]";
    }
    public record EntityChange(Entity UpdatedEntity) : StateChange
    {
        public override string ToString() => $"[{base.ToString()}: {UpdatedEntity.ToString()}]";
    }
}

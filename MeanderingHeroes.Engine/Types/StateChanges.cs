using MeanderingHeroes.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public abstract record StateChange;
    public record LayerItemChange(LayerItem OldItem, LayerItem NewItem) : StateChange;
    public record EntityChange(Entity UpdatedEntity) : StateChange;
}

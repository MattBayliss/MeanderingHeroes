using MeanderingHeroes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes
{
    /// <summary>
    /// contains the data that can change each tick 
    /// </summary>
    public record GameState
    {
        // later: segregate into hexes : Dict<Hex, List<Entities>>
        public ImmutableList<Entity> Entities { get; init; }

        public GameState(IEnumerable<Entity> entities)
        {
            Entities = entities.ToImmutableList();
        }
    }
}

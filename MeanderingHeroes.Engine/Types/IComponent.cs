using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public interface IComponent
    {
        GameState Update2(Game game, GameState state);
    }
}

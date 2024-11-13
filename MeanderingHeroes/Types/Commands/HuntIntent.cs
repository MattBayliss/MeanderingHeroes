using MeanderingHeroes.Types.Doers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MeanderingHeroes.ModelLibrary;

namespace MeanderingHeroes.Types.Commands
{
    public record HuntIntent : DoerIntent
    {
        private HuntIntent(int doerId) : base(doerId)
        {
        }

        public static HuntIntent Create(int doerId)
        {
            return new HuntIntent(doerId);
        }
    }

    public static partial class ModelLibrary
    {
        public static GameState AddHuntIntent(this GameState state, Doer doer) =>
            state with { Commands = state.Commands.Add(HuntIntent.Create(doer.Id)) };
    }
}

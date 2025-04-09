using MeanderingHeroes.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public static class EntityFactory
    {
        private static int LastId { get; set; }

        public static void SetLastId(int lastId)
        {
            LastId = lastId;
        }

        public static SmartEntity CreateSmartEntity(FractionalHex hexCoords, float speed) => new SmartEntity(LastId++, hexCoords, speed);
        public static Advertiser CreateAdvertiser(FractionalHex hexCoords, IEnumerable<Offer> offers) => new Advertiser(LastId++, hexCoords, offers);
    }
}

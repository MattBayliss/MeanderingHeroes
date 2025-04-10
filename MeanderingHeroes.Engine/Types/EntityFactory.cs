using MeanderingHeroes.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    internal class EntityFactory
    {
        private int _lastId;

        public EntityFactory(int lastId)
        {
            _lastId = lastId;
        }

        public SmartEntity CreateSmartEntity(FractionalHex hexCoords, float speed) => new SmartEntity(_lastId++, hexCoords, speed);
        public Advertiser CreateAdvertiser(FractionalHex hexCoords, IEnumerable<Offer> offers) => new Advertiser(_lastId++, hexCoords, offers);
    }
}

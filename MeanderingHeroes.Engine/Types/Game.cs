using MeanderingHeroes.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeanderingHeroes.Engine.Types
{
    public class Game
    {
        public Grid HexMap { get; init; }
        public Transforms Transforms { get; init; }
        public IEnumerable<Entity> Entities { get; private set; }
        private EntityFactory _entityFactory;
        public Game(Grid hexMap, Transforms transforms)
        {
            HexMap = hexMap;
            Transforms = transforms;
            Entities = [];
            _entityFactory = new EntityFactory(0);
        }
        public Game(Grid hexMap, Transforms transforms, IEnumerable<Entity> entities)
        {
            HexMap = hexMap;
            Transforms = transforms;
            Entities = entities;
            _entityFactory = new EntityFactory(entities.Select(e => e.Id).Append(0).Max());
        }
        private T CreateEntityAndAppendToEntities<T>(Func<T> entityCreator) where T : Entity
        {
            var newEntity = entityCreator();
            Entities = Entities.Append(newEntity);
            return newEntity;
        }
        public SmartEntity CreateSmartEntity(FractionalHex hexCoords, float speed)
            => CreateEntityAndAppendToEntities(() 
                => _entityFactory.CreateSmartEntity(hexCoords, speed));
        public Advertiser CreateAdvertiser(FractionalHex hexCoords, IEnumerable<Offer> offers)
            => CreateEntityAndAppendToEntities(()
                => _entityFactory.CreateAdvertiser(hexCoords, offers));
    }
}

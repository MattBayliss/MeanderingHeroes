namespace MeanderingHeroes.Engine.Types
{
    internal class EntityFactory
    {
        private int _lastId;

        public EntityFactory(int lastId)
        {
            _lastId = lastId;
        }

        public SmartEntity CreateSmartEntity(FractionalHex hexCoords, float speed) 
            => new SmartEntity(_lastId++, hexCoords, speed);
    }
}

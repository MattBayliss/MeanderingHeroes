namespace MeanderingHeroes.Engine.Types
{
    internal class EntityFactory
    {
        private int _lastId;

        public EntityFactory(int lastId)
        {
            _lastId = lastId;
        }

        public Entity CreateEntity(FractionalHex hexCoords, float speed) 
            => new Entity(_lastId++, hexCoords, speed);
    }
}

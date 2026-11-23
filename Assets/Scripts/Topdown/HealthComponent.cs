namespace Bas.Thoth.Topdown
{
    public class HealthComponent : Component<Health>
    {
        private Entities _entities;
        public override void Init()
        {
            _entities = Singletons.Get<Entities>();
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            if (this[entityID].health <= 0)
            {
                _entities.DestroyEntity(entityID);
            }
        }
    }
}

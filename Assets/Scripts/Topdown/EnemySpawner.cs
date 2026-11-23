namespace Bas.Thoth.Topdown
{
    public class EnemySpawner : Component<EnemySpawn>
    {
        private TopdownFactory _factory;
        private Entities _entities;

        public override void Init()
        {
            _factory = Singletons.Get<TopdownFactory>();
            _entities = Singletons.Get<Entities>();
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            EnemySpawn spawn = this[entityID];
            spawn.spawnTimer -= deltaTime;
            if (spawn.spawnTimer <= 0)
            {
                spawn.spawnTimer += 1 / spawn.rate;
                _factory.CreateEnemy(spawn.x, spawn.y);
                spawn.count++;
                if (spawn.count >= spawn.max)
                {
                    _entities.DestroyEntity(entityID);
                }
            }
            this[entityID] = spawn;
        }
    }
}

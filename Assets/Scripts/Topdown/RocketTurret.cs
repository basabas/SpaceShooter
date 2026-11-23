using Bas.Debug;
using Bas.Thoth.Models;
using System.Collections.Generic;

namespace Bas.Thoth.Topdown
{
    public class RocketTurret : Component<RocketTurretInformation>, IOnCollision
    {
        private Dictionary<int, HashSet<int>> _enemiesInRange = new Dictionary<int, HashSet<int>>();

        private TransformComponent _transforms;
        private TopdownFactory _factory;
        private OverlapHandler _overlapHandler;
        private Enemy _enemies;
        private HealthComponent _healthComponent;

        public override void Init()
        {
            _transforms = Singletons.Get<TransformComponent>();
            _factory = Singletons.Get<TopdownFactory>();
            _overlapHandler = Singletons.Get<OverlapHandler>();
            _enemies = Singletons.Get<Enemy>();
            _healthComponent = Singletons.Get<HealthComponent>();
        }

        public void OnCollision(CollisionResult collisionResult)
        {
            if (_enemiesInRange.TryGetValue(collisionResult.id, out HashSet<int> enemies))
            {
                if (_enemies.HasEntity(collisionResult.otherId))
                {
                    enemies.Add(collisionResult.otherId);
                }
            }
            else if (_enemiesInRange.TryGetValue(collisionResult.otherId, out enemies))
            {
                if (_enemies.HasEntity(collisionResult.id))
                {
                    enemies.Add(collisionResult.id);
                }
            }
            else
            {
                Logging.LogError("Why are we here???>");
            }
        }

        public override void AddEntity(int entityID)
        {
            _enemiesInRange.Add(entityID, new HashSet<int>());
            base.AddEntity(entityID);
            _overlapHandler.Subscribe(entityID, this);
        }

        public override bool RemoveEntity(int entityID)
        {
            _overlapHandler.UnSubscribe(entityID, this);
            return _enemiesInRange.Remove(entityID) && base.RemoveEntity(entityID);
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            RocketTurretInformation turret = this[entityID];
            Transform transform = _transforms[entityID];

            if (turret.fireTime <= 0)
            {
                foreach (int targetID in _enemiesInRange[entityID])
                {
                    Health health = _healthComponent[targetID];

                    if (health.virtualHealth > 0)
                    {
                        _factory.CreateMissile(transform.x, transform.y, targetID, 4f, turret.damage);
                        turret.fireTime += 1f / turret.fireSpeed;
                        health.virtualHealth -= turret.damage;
                        _healthComponent[targetID] = health;
                        break;
                    }
                }
            }
            _enemiesInRange[entityID].Clear();
            turret.fireTime -= deltaTime;
            this[entityID] = turret;

            Transform playerTransform = _transforms[Player.EntityID];
            transform.x = turret.x + playerTransform.x;
            transform.y = turret.y + playerTransform.y;
            _transforms[entityID] = transform;

        }
    }
}

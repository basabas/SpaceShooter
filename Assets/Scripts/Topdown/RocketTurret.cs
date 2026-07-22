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

        public override void Init()
        {
            _transforms = Singletons.Get<TransformComponent>();
            _factory = Singletons.Get<TopdownFactory>();
            _overlapHandler = Singletons.Get<OverlapHandler>();
        }

        public void OnCollision(CollisionResult collisionResult)
        {
            if (_enemiesInRange.TryGetValue(collisionResult.id, out HashSet<int> enemies))
            {
                enemies.Add(collisionResult.otherId);
            }
            else if (_enemiesInRange.TryGetValue(collisionResult.otherId, out enemies))
            {
                enemies.Add(collisionResult.id);
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

            if (turret.fireTime <= 0)
            {
                Transform transform = _transforms[entityID];
                foreach (int targetID in _enemiesInRange.Keys)
                {
                    Transform targetTransform = _transforms[targetID];

                    if (Transform.Distance(transform, targetTransform) < turret.range)
                    {
                        Shoot(targetID, transform);
                    }
                }
            }

            turret.fireTime -= deltaTime;
            this[entityID] = turret;
        }

        public void Shoot(int targetID, Transform transform)
        {
            _factory.CreateMissile(transform.x, transform.y, targetID, 1f);
        }
    }
}

using Bas.Thoth.Models;

namespace Bas.Thoth.Topdown
{
    public class Missile : Component<MissileInformation>
    {
        private const float _maxAcceleration = 400;//TODO: Should be velocity

        private TransformComponent _transformComponent;
        private Physics _physicsComponent;
        private Entities _entities;
        private HealthComponent _healthComponent;

        public override void Init()
        {
            _transformComponent = Singletons.Get<TransformComponent>();
            _physicsComponent = Singletons.Get<Physics>();
            _entities = Singletons.Get<Entities>();
            _healthComponent = Singletons.Get<HealthComponent>();
        }

        public override void UpdateEntity(int id, float deltaTime)
        {
            MissileInformation missile = this[id];
            int targetId = missile.target;
            if (!_entities.IsAlive(targetId))
            {
                _entities.DestroyEntity(id);
                return;
            }

            Transform target = _transformComponent[targetId];
            Transform transform = _transformComponent[id];
            PhysicsInformation physics = _physicsComponent[id];

            float deltaX = target.x - transform.x;
            float deltaY = target.y - transform.y;

            float delta = Math.GetSquaredDistance(transform.x, transform.y, target.x, target.y);

            //if (_overlapResolver.Overlaps(id, targetId, out CollisionResult _))
            if (delta < 1)
            {
                //TODO: spawn explosion or something

                Health health = _healthComponent[targetId];
                health.health -= missile.damage;
                _healthComponent[targetId] = health;
                _entities.DestroyEntity(id);
                return;
            }

            transform.rotation = Math.GetAngle(deltaX, deltaY);

            delta = Math.Sqrt(delta);

            physics.xVelocity = deltaX / delta * _maxAcceleration;
            physics.yVelocity = deltaY / delta * _maxAcceleration;

            _transformComponent[id] = transform;
            _physicsComponent[id] = physics;
        }
    }
}

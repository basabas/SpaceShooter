using Bas.Thoth.Models;

namespace Bas.Thoth.Topdown
{
    [Priority(Priority.Rest)]
    public class Enemy : Component
    {
        private const float _maxAcceleration = 250;

        private TransformComponent _transformComponent;
        private Physics _physicsComponent;
        private Entities _entities;
        private OverlapResolver _overlapResolver;

        public override void Init()
        {
            _transformComponent = Singletons.Get<TransformComponent>();
            _physicsComponent = Singletons.Get<Physics>();
            _entities = Singletons.Get<Entities>();
            _overlapResolver = Singletons.Get<OverlapResolver>();
        }

        public override void UpdateEntity(int id, float deltaTime)
        {
            Transform target = _transformComponent[Player.EntityID];
            Transform transform = _transformComponent[id];
            PhysicsInformation physics = _physicsComponent[id];

            float deltaX = target.x - transform.x;
            float deltaY = target.y - transform.y;

            float delta = Math.GetSquaredDistance(transform.x, transform.y, target.x, target.y);

            //TODO: Actually write some AI logic so we don't just fly straight at the player till an arbitrary number 
            if (delta < 400 * 400)
            {
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

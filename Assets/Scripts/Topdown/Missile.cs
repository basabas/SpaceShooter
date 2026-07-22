using Bas.Thoth.Models;

namespace Bas.Thoth.Topdown
{
    public class Missile : Component, IOnCollision
    {
        private const float _maxAcceleration = 250;

        private OverlapHandler _overlapHandler;
        private TransformComponent _transformComponent;
        private Physics _physicsComponent;
        private Entities _entities;

        public override void Init()
        {
            _transformComponent = Singletons.Get<TransformComponent>();
            _physicsComponent = Singletons.Get<Physics>();
            _overlapHandler = Singletons.Get<OverlapHandler>();
            _entities = Singletons.Get<Entities>();
        }

        void IOnCollision.OnCollision(CollisionResult collisionResult)
        {
            if (collisionResult.id == Player.EntityID)
            {
                //TODO: Inflict some damage, health, armor or something
                _entities.DestroyEntity(collisionResult.otherId);
            }
            if (collisionResult.otherId == Player.EntityID)
            {
                //TODO: Inflict some damage, health, armor or something
                _entities.DestroyEntity(collisionResult.id);
            }
        }

        public override void UpdateEntity(int id, float deltaTime)
        {
            Transform player = _transformComponent[Player.EntityID];
            Transform transform = _transformComponent[id];
            PhysicsInformation physics = _physicsComponent[id];

            float deltaX = player.x - transform.x;
            float deltaY = player.y - transform.y;

            float delta = UnityEngine.Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (delta < 100)
            {
                _entities.DestroyEntity(id);
                return;
            }

            //physics.xAcceleration = deltaX / length * _maxAcceleration;
            //physics.yAcceleration = deltaY / length * _maxAcceleration;

            transform.rotation = UnityEngine.Mathf.Deg2Rad * UnityEngine.Vector2.SignedAngle(UnityEngine.Vector2.up, new UnityEngine.Vector2(physics.xVelocity, physics.yVelocity));

            physics.xVelocity = deltaX / delta * _maxAcceleration;
            physics.yVelocity = deltaY / delta * _maxAcceleration;

            _transformComponent[id] = transform;
            _physicsComponent[id] = physics;
        }

        public override void AddEntity(int entityID)
        {
            base.AddEntity(entityID);
            _overlapHandler.Subscribe(entityID, this);
        }

        public override bool RemoveEntity(int entityID)
        {
            _overlapHandler.UnSubscribe(entityID, this);
            return base.RemoveEntity(entityID);
        }
    }
}

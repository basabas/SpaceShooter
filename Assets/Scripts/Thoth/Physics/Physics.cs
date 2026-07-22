using Bas.Thoth.Models;

namespace Bas.Thoth
{
    public class Physics : Component<PhysicsInformation>
    {
        private TransformComponent _transformComponent;

        public override void Init() => _transformComponent = Singletons.Get<TransformComponent>();

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            Transform transform = _transformComponent[entityID];
            PhysicsInformation physics = this[entityID];

            physics.xVelocity += physics.xAcceleration * deltaTime * 0.5f;
            physics.yVelocity += physics.yAcceleration * deltaTime * 0.5f;
            physics.rotVelocity += physics.rotAcceleration * deltaTime * 0.5f;

            float dragFactor = 1f / (1f + physics.linearDrag * deltaTime);
            physics.xVelocity *= dragFactor;
            physics.yVelocity *= dragFactor;
            physics.rotVelocity *= dragFactor;

            transform.x += physics.xVelocity * deltaTime;
            transform.y += physics.yVelocity * deltaTime;
            transform.rotation += physics.rotVelocity * deltaTime;

            physics.xVelocity += physics.xAcceleration * deltaTime * 0.5f;
            physics.yVelocity += physics.yAcceleration * deltaTime * 0.5f;
            physics.rotVelocity += physics.rotAcceleration * deltaTime * 0.5f;

            // Boundary checks, TEMPORARY
            if (transform.x + transform.scale > World.Width)
            {
                transform.x = World.Width - transform.scale;
                physics.xVelocity = -physics.xVelocity;
            }
            else if (transform.x - transform.scale < -World.Width)
            {
                transform.x = -World.Width + transform.scale;
                physics.xVelocity = -physics.xVelocity;
            }

            if (transform.y + transform.scale > World.Height)
            {
                transform.y = World.Height - transform.scale;
                physics.yVelocity = -physics.yVelocity;
            }
            else if (transform.y - transform.scale < -World.Height)
            {
                transform.y = -World.Height + transform.scale;
                physics.yVelocity = -physics.yVelocity;
            }

            physics.xAcceleration = 0;
            physics.xAcceleration = 0;

            _transformComponent[entityID] =  transform;
            this[entityID] = physics;
        }
    }
}
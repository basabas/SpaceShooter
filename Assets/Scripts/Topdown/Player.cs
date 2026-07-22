using Bas.Thoth.Models;
using UnityEngine.InputSystem;

namespace Bas.Thoth.Topdown
{
    public class Player : Component
    {
        public static int EntityID { get; private set; }

        private const float acceleration = 1000;

        private Physics _physicsComponent;

        public override void Init() => _physicsComponent = Singletons.Get<Physics>();

        public override void AddEntity(int id)
        {
            base.AddEntity(id);
            EntityID = id;
        }

        public override void UpdateEntity(int id, float deltaTime)
        {
            PhysicsInformation physics = _physicsComponent[id];

            physics.xVelocity += Keyboard.current.dKey.isPressed ? acceleration * deltaTime : 0;
            physics.yVelocity -= Keyboard.current.sKey.isPressed ? acceleration * deltaTime : 0;
            physics.xVelocity -= Keyboard.current.aKey.isPressed ? acceleration * deltaTime : 0;
            physics.yVelocity += Keyboard.current.wKey.isPressed ? acceleration * deltaTime : 0;

            _physicsComponent[id] = physics;
        }
    }
}

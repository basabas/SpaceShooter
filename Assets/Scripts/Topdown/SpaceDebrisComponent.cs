using Bas.Thoth.Models;

namespace Bas.Thoth.Topdown
{
    public class SpaceDebrisComponent : Component
    {
        private TransformComponent _transformComponent;

        public override void Init()
        {
            _transformComponent = Singletons.Get<TransformComponent>();
        }

        public override void UpdateEntity(int id, float deltaTime)
        {
            Transform player = _transformComponent[Player.EntityID];
            Transform transform = _transformComponent[id];

            float right = player.x + World.CameraMaxWidth * .6f;
            float left = player.x - World.CameraMaxWidth * .6f;
            float up = player.y + World.CameraMaxWidth * .6f;
            float down = player.y - World.CameraMaxWidth * .6f;

            if (transform.x > right)
            {
                transform.x = left;
            }
            else if (transform.x < left)
            {
                transform.x = right;
            }

            if (transform.y > up)
            {
                transform.y = down;
            }
            else if (transform.y < down)
            {
                transform.y = up;
            }

            _transformComponent[id] = transform;
        }
    }
}

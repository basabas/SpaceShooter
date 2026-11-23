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
            float camSize = World.CameraMaxWidth * .6f;
            float right = player.x + camSize;
            float left = player.x - camSize;
            float up = player.y + camSize;
            float down = player.y - camSize;

            if (transform.x > right)
            {
                transform.x -= camSize * 2;
            }
            else if (transform.x < left)
            {
                transform.x += camSize * 2;
            }

            if (transform.y > up)
            {
                transform.y -= camSize * 2;
            }
            else if (transform.y < down)
            {
                transform.y += camSize * 2;
            }

            _transformComponent[id] = transform;
        }
    }
}

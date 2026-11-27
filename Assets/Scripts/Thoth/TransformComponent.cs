using Bas.Thoth.Models;

namespace Bas.Thoth
{
    [Priority(Priority.Rest)]
    public class TransformComponent : Component<Transform>
    {
        public override void Update(float deltaTime)
        {
            //We dont want to do anything with TransformComponent
        }
        public override void UpdateEntity(int entityID, float deltaTime)
        {
            //We dont want to do anything with TransformComponent
        }
    }
}

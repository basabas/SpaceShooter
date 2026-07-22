using Bas.Thoth.Models;

namespace Bas.Thoth
{
    public class PhysicsResolver
    {
        public TransformComponent _transforms;
        public Physics _physics;

        public PhysicsResolver()
        {
            _transforms = Singletons.Get<TransformComponent>();
            _physics = Singletons.Get<Physics>();
        }

        public void Resolve(CollisionResult result)
        {
            int a = result.id;
            int b = result.otherId;
            Transform aTransform = _transforms[a];
            Transform bTransform = _transforms[b];

            // Circle - Circle
            if (result.collisionType == CollisionType.CircleCircle)
            {
                ResolveCircleCircle(a, b, aTransform, bTransform, result);
                return;
            }

            // Circle - Rect (both orders)
            if (result.collisionType == CollisionType.CircleRect)
            {
                ResolveCircleRect(a, b, aTransform, bTransform, result);
                return;
            }

            if (result.collisionType == CollisionType.RectCircle)
            {
                ResolveCircleRect(a, b, aTransform, bTransform, result);
                return;
            }

            // Rect - Rect
            if (result.collisionType == CollisionType.RectRect)
            {
                ResolveRectRect(a, b, aTransform, bTransform, result);
                return;
            }

            // Custom or unsupported shapes: no physics response yet.
        }

        private void ResolveCircleCircle(int a, int b, Transform aTransform, Transform bTransform, CollisionResult result)
        {
            PhysicsInformation aPhysics = _physics[a];
            PhysicsInformation bPhysics = _physics[b];

            float distance = Math.Sqrt(result.distanceSq);

            float overlap = 0.5f * (distance - result.maxDistance);

            aTransform.x += overlap * result.differenceX / distance;
            aTransform.y += overlap * result.differenceY / distance;

            bTransform.x -= overlap * result.differenceX / distance;
            bTransform.y -= overlap * result.differenceY / distance;

            distance = result.maxDistance;

            float normalXDistance = (bTransform.x - aTransform.x) / distance;
            float normalYDistance = (bTransform.y - aTransform.y) / distance;

            float velocityXDifference = aPhysics.xVelocity - bPhysics.xVelocity;
            float velocityYDifference = aPhysics.yVelocity - bPhysics.yVelocity;

            float totalMass = aPhysics.mass + bPhysics.mass;

            _transforms[a] = aTransform;
            _transforms[b] = bTransform;

            if (totalMass <= 0f)
            {
                return;
            }

            float collisionScale = 2.0f * (normalXDistance * velocityXDifference + normalYDistance * velocityYDifference) / totalMass;

            aPhysics.xVelocity -= collisionScale * bPhysics.mass * normalXDistance;
            aPhysics.yVelocity -= collisionScale * bPhysics.mass * normalYDistance;
            bPhysics.xVelocity += collisionScale * aPhysics.mass * normalXDistance;
            bPhysics.yVelocity += collisionScale * aPhysics.mass * normalYDistance;

            _physics[a] = aPhysics;
            _physics[b] = bPhysics;
        }



        private void ResolveCircleRect(int a, int b, Transform aTransform, Transform bTransform, CollisionResult result)
        {
            float move = result.penetration * 0.5f;
            aTransform.x += result.normalX * move;
            aTransform.y += result.normalY * move;
            bTransform.x -= result.normalX * move;
            bTransform.y -= result.normalY * move;

            _transforms[a] = aTransform;
            _transforms[b] = bTransform;

            PhysicsInformation aPhysics = _physics[a];
            PhysicsInformation bPhysics = _physics[b];

            float relVelX = aPhysics.xVelocity - bPhysics.xVelocity;
            float relVelY = aPhysics.yVelocity - bPhysics.yVelocity;

            float totalMass = aPhysics.mass + bPhysics.mass;
            if (totalMass <= 0f)
            {
                return;
            }

            float collisionScale = 2.0f * (result.normalX * relVelX + result.normalY * relVelY) / totalMass;

            aPhysics.xVelocity -= collisionScale * bPhysics.mass * result.normalX;
            aPhysics.yVelocity -= collisionScale * bPhysics.mass * result.normalY;
            bPhysics.xVelocity += collisionScale * aPhysics.mass * result.normalX;
            bPhysics.yVelocity += collisionScale * aPhysics.mass * result.normalY;

            _physics[a] = aPhysics;
            _physics[b] = bPhysics;
        }


        private void ResolveRectRect(int a, int b, Transform aTransform, Transform bTransform, CollisionResult result)
        {
            float move = result.penetration * 0.5f;
            aTransform.x -= result.normalX * move;
            aTransform.y -= result.normalY * move;
            bTransform.x += result.normalX * move;
            bTransform.y += result.normalY * move;

            _transforms[a] = aTransform;
            _transforms[b] = bTransform;

            PhysicsInformation aPhysics = _physics[a];
            PhysicsInformation bPhysics = _physics[b];

            float relVelX = aPhysics.xVelocity - bPhysics.xVelocity;
            float relVelY = aPhysics.yVelocity - bPhysics.yVelocity;

            float totalMass = aPhysics.mass + bPhysics.mass;
            if (totalMass <= 0f)
            {
                return;
            }

            float collisionScale = 2.0f * (result.normalX * relVelX + result.normalY * relVelY) / totalMass;

            aPhysics.xVelocity -= collisionScale * bPhysics.mass * result.normalX;
            aPhysics.yVelocity -= collisionScale * bPhysics.mass * result.normalY;
            bPhysics.xVelocity += collisionScale * aPhysics.mass * result.normalX;
            bPhysics.yVelocity += collisionScale * aPhysics.mass * result.normalY;

            _physics[a] = aPhysics;
            _physics[b] = bPhysics;
        }
    }
}
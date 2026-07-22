using Bas.Thoth;
using Bas.Thoth.Models;

namespace Bas.Thoth
{
    public class OverlapResolver
    {
        private const float MinDistanceSq = 0.0001f;

        public Collision _collision;
        public TransformComponent _transforms;

        public OverlapResolver(Collision collision)
        {
            _collision = collision;
            _transforms = Singletons.Get<TransformComponent>();
        }

        public void UpdateBounds(int entityID)
        {
            Collider collider = _collision[entityID];
            Transform transform = _transforms[entityID];
            switch (collider.colliderShape)
            {
                case ColliderShape.Circle:
                default:
                    {
                        float radius = collider.radius * transform.scale;
                        collider.MinX = transform.x - radius;
                        collider.MaxX = transform.x + radius;
                        collider.MinY = transform.y - radius;
                        collider.MaxY = transform.y + radius;
                        break;
                    }
                case ColliderShape.Rectangle:
                    {
                        float halfWidth = collider.width * 0.5f * transform.scale;
                        float halfHeight = collider.height * 0.5f * transform.scale;
                        collider.MinX = transform.x - halfWidth;
                        collider.MaxX = transform.x + halfWidth;
                        collider.MinY = transform.y - halfHeight;
                        collider.MaxY = transform.y + halfHeight;
                        break;
                    }
            }

            _collision[entityID] = collider;
        }

        public bool AabbOverlap(Collider a, Collider b) =>
            a.MinX <= b.MaxX &&
            a.MaxX >= b.MinX &&
            a.MinY <= b.MaxY &&
            a.MaxY >= b.MinY;

        public bool Overlaps(Collider aCollider, Collider bCollider, Transform aTransform, Transform bTransform, ref CollisionResult result)
        {
            // Circle - Circle
            if (aCollider.colliderShape == ColliderShape.Circle && bCollider.colliderShape == ColliderShape.Circle)
            {
                return CircleCircleOverlap(aCollider, bCollider, aTransform, bTransform, ref result);
            }

            // Circle - Rect (both orders)
            if (aCollider.colliderShape == ColliderShape.Circle && bCollider.colliderShape == ColliderShape.Rectangle)
            {
                return CircleRectOverlap(aCollider, bCollider, aTransform, bTransform, ref result);
            }

            if (bCollider.colliderShape == ColliderShape.Circle && aCollider.colliderShape == ColliderShape.Rectangle)
            {
                return CircleRectOverlap(aCollider, bCollider, aTransform, bTransform, ref result);
            }

            // Rect - Rect
            if (aCollider.colliderShape == ColliderShape.Rectangle && bCollider.colliderShape == ColliderShape.Rectangle)
            {
                return RectRectOverlap(aCollider, bCollider, aTransform, bTransform, ref result);
            }

            // Custom or unsupported shapes: treat as no overlap for now.
            return false;
        }



        private bool CircleCircleOverlap(Collider aCollider, Collider bCollider, Transform aTransform, Transform bTransform, ref CollisionResult result)
        {
            float radiusA = aCollider.radius * aTransform.scale;
            float radiusB = bCollider.radius * bTransform.scale;

            result.maxDistance = radiusA + radiusB;

            result.differenceX = bTransform.x - aTransform.x;
            result.differenceY = bTransform.y - aTransform.y;
            result.distanceSq = result.differenceX * result.differenceX + result.differenceY * result.differenceY;

            if (result.distanceSq <= MinDistanceSq || result.distanceSq >= result.maxDistance * result.maxDistance)
            {
                return false;
            }

            result.x = aTransform.x + result.differenceX * (radiusA / (radiusA + radiusB));
            result.y = bTransform.y + result.differenceY * (radiusA / (radiusA + radiusB));
            return true;
        }

        private bool CircleRectOverlap(Collider aCollider, Collider bCollider, Transform aTransform, Transform bTransform, ref CollisionResult result)
        {
            float radius = aCollider.radius * aTransform.scale;

            float halfWidth = bCollider.width * 0.5f * bTransform.scale;
            float halfHeight = bCollider.height * 0.5f * bTransform.scale;

            float rectCenterX = bTransform.x;
            float rectCenterY = bTransform.y;

            result.differenceX = aTransform.x - rectCenterX;
            result.differenceY = aTransform.y - rectCenterY;

            result.x = rectCenterX + UnityEngine.Mathf.Clamp(result.differenceX, -halfWidth, halfWidth);
            result.y = rectCenterY + UnityEngine.Mathf.Clamp(result.differenceY, -halfHeight, halfHeight);

            result.normalX = aTransform.x - result.x;
            result.normalY = aTransform.y - result.y;
            result.distanceSq = result.normalX * result.normalX + result.normalY * result.normalY;

            if (result.distanceSq <= MinDistanceSq || result.distanceSq >= radius * radius)
            {
                return false;
            }

            float dist = UnityEngine.Mathf.Sqrt(result.distanceSq);
            result.penetration = radius - dist;
            if (result.penetration <= 0f)
            {
                return false;
            }

            float invDist = 1.0f / dist;
            result.normalX *= invDist;
            result.normalY *= invDist;

            return true;
        }

        private static bool RectRectOverlap(Collider aCollider, Collider bCollider, Transform aTransform, Transform bTransform, ref CollisionResult result)
        {
            float halfWidthA = aCollider.width * 0.5f * aTransform.scale;
            float halfHeightA = aCollider.height * 0.5f * aTransform.scale;

            float halfWidthB = bCollider.width * 0.5f * bTransform.scale;
            float halfHeightB = bCollider.height * 0.5f * bTransform.scale;

            result.differenceX = bTransform.x - aTransform.x;
            result.differenceY = bTransform.y - aTransform.y;

            float overlapX = halfWidthA + halfWidthB - UnityEngine.Mathf.Abs(result.differenceX);
            float overlapY = halfHeightA + halfHeightB - UnityEngine.Mathf.Abs(result.differenceY);

            if (overlapX <= 0f || overlapY <= 0f)
            {
                return false;
            }

            if (overlapX < overlapY)
            {
                result.penetration = overlapX;
                result.normalX = result.differenceX > 0f ? 1f : -1f;
                result.normalY = 0f;
            }
            else
            {
                result.penetration = overlapY;
                result.normalX = 0f;
                result.normalY = result.differenceY > 0f ? 1f : -1f;
            }


            result.x = aTransform.x + result.differenceX * (halfWidthA / (halfWidthA + halfWidthB));
            result.y = bTransform.y + result.differenceY * (halfHeightA / (halfHeightA + halfHeightB));

            return true;
        }
    }
}
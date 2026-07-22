namespace Bas.Thoth
{
    public struct CollisionResult
    {
        public int id;
        public int otherId;
        public CollisionType collisionType;
        public float differenceX;
        public float differenceY;
        public float distanceSq;
        public float normalX;
        public float normalY;
        public float maxDistance;
        public float penetration;
        public float x;
        public float y;

        public override string ToString()
        {
            return $"id:{id}, otherId:{otherId}, collisionType:{collisionType} and others....";
        }
    }

    public enum CollisionType
    {
        CircleCircle,
        CircleRect,
        RectCircle,
        RectRect
    }
}
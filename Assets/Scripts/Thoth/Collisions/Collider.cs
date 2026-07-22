namespace Bas.Thoth.Models
{
    public struct Collider
    {
        public float radius;
        public float width;
        public float height;
        public ColliderShape colliderShape;
        public ColliderType colliderType;

        public float MinX;
        public float MaxX;
        public float MinY;
        public float MaxY;

        public override string ToString() => $"Collider[radius:{radius}, width:{width}, height:{height}, colliderShaper:{colliderShape}, physicsType:{colliderType}]";
    }
}
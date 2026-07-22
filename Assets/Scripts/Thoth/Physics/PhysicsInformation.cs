namespace Bas.Thoth.Models
{
    public struct PhysicsInformation
    {
        public float xVelocity;
        public float yVelocity;
        public float xAcceleration;
        public float yAcceleration;
        public float rotVelocity;
        public float rotAcceleration;
        public float mass;
        public float bounciness;
        public float linearDrag;

        public override string ToString() => $"Physics[xVel:{xVelocity}, yVel:{yVelocity}, xAcc:{xAcceleration}, yAcc:{yAcceleration}, rotVel:{rotVelocity}, rotAcc:{rotAcceleration}, mass:{mass}, bounce:{bounciness}, linearDrag:{linearDrag}]";
    }
}
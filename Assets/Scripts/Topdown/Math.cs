namespace Bas.Thoth
{
    public static class Math
    {
        public static float GetAngle(float x, float y) => (float)System.Math.Atan2(-x, y);

        public static float GetLenght(float x, float y) => (float)System.Math.Sqrt(x * x + y * y);

        public static float GetDistance(float x1, float y1, float x2, float y2)
        {
            float deltaX = x2 - x1;
            float deltaY = y2 - y1;
            return (float)System.Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        public static (float, float) GetNormalized(float x, float y)
        {
            float length = GetLenght(x, y);
            if (length <= 0f)
            {
                return (0f, 0f);
            }
            return (x / length, y / length);
        }
    }
}
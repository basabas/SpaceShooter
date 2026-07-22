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

        public static float Abs(float x) => x < 0 ? -x : x;

        public static float Sqrt(float x) => (float)System.Math.Sqrt(x);

        public static int Clamp(int value, int min, int max) => value < min ? min : (value > max ? max : value);

        public static float Clamp(float value, float min, float max) => value < min ? min : (value > max ? max : value);
        public static float Clamp01(float value) => value < 0 ? 0 : (value > 1 ? 1 : value);

        public static int Max(int value, int v2) => value > v2 ? value : v2;
        public static float Max(float value, float v2) => value > v2 ? value : v2;

        public static int Max(params int[] values)
        {
            int max = int.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                int v = values[i];
                if (v > max)
                {
                    max = v;
                }
            }
            return max;
        }

        public static float Max(params float[] values)
        {
            float max = float.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                float v = values[i];
                if (v > max)
                {
                    max = v;
                }
            }
            return max;
        }

        public static int CeilToInt(float x) => ((int)x) + 1;
        public static int FloorToInt(float x) => (int)x;

        public static float Pow(float x, int y) => (float)System.Math.Pow(x, y);

        public static float Lerp(float start, float goal, float progress) => start + (goal - start) * progress;
        public static float InverseLerp(float a, float b, float value) => a == b ? 0 : Clamp01((value - a) / (b - a));
    }
}
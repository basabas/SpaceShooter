using System;

namespace Bas.Thoth.Models
{
    public struct Transform
    {
        public float x;
        public float y;
        public float rotation;
        public float scale;

        public static float Distance(Transform a, Transform b)
        {
            float x1 = a.x;
            float x2 = b.x;
            float y1 = a.y;
            float y2 = b.y;
            return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public override string ToString() => $"Transform[x:{x}, y:{y}, rot:{rotation}, scale:{scale}]";
    }
}
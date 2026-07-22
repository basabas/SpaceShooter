namespace Bas.Thoth.Models
{
    public struct Transform
    {
        public float x;
        public float y;
        public float rotation;
        public float scale;

        public override string ToString() => $"Transform[x:{x}, y:{y}, rot:{rotation}, scale:{scale}]";
    }
}
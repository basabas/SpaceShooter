namespace Bas.Thoth.Models
{
    public struct FrameInformation
    {
        public float fps;
        public int frameStart;
        public int frameCount;
        public float width;
        public float height;

        public override string ToString() => $"Frame[fps:{fps}, frameStart:{frameStart}, frameCount:{frameCount}, width:{width}, height:{height}]";
    }
}
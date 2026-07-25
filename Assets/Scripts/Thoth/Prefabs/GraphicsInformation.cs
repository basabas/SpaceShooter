using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bas.Thoth
{
    [Serializable]
    public class GraphicsInformation
    {
        [Range(0, 128)]
        public float fps;
        public List<Vector4> rects { get; } = new();
        public List<Sprite> frames = new();
        public float width;
        public float height;
    }
}

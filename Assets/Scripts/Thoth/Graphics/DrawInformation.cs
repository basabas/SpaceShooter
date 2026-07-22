using UnityEngine;

namespace Bas.Thoth.Models
{
    public struct DrawInformation
    {
        public int textureIndex;
        public Color color;

        public override string ToString() => $"Draw[textureIndex:{textureIndex}, color:{color}]";
    }
}
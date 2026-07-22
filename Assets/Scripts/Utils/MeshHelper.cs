using UnityEngine;

namespace Bas.Utils
{
    public static class MeshHelper
    {
        public static Mesh GetQuadMesh(float width, float height) => new Mesh
        {
            vertices = new[]
            {
            new Vector3(-width / 2, -height / 2, 0),
            new Vector3(width / 2, -height / 2, 0),
            new Vector3(-width / 2, height / 2, 0),
            new Vector3(width / 2, height / 2, 0)
        },

            triangles = new[] { 0, 2, 1, 2, 3, 1 },

            uv = new[]
            {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        }
        };
    }
}
using Bas.Debug;
using UnityEngine;

namespace Bas.Thoth.Topdown
{
    public class RandomColorPicker
    {
        private Texture2D _texture;
        private Color[] _colors;

        private float _rotation;

        public RandomColorPicker(string resourcePath)
        {
            _texture = Resources.Load<Texture2D>(resourcePath);
            _colors = _texture.GetPixels();
            _rotation = Random.Range(0, 360f);
        }

        public Color GetColor(float normalizedX, float normalizedY)
        {
            (float rotatedX, float rotatedY) = Math.RotatePoint(normalizedX, normalizedY, .5f, .5f, _rotation);
            int x = Math.FloorToInt(_texture.width * Math.Clamp01(rotatedX));
            int y = Math.FloorToInt(_texture.height * Math.Clamp01(rotatedY));
            int index = x + y * _texture.width;
            if (index >= _colors.Length)
            {
                Logging.LogError($"Index out of bounds: {index} >= {_colors.Length} (x: {x}, y: {y}, width: {_texture.width}, height: {_texture.height})");
            }
            return _colors[index];
        }

        public void Dispose()
        {
            if (_texture)
            {
                Object.Destroy(_texture);
            }
        }
    }

}

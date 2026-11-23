using Bas.Thoth.Models;

namespace Bas.Thoth
{
    public class DynamicGrid
    {
        private static readonly int _maxSize = 400;

        public GridArea this[int index] => _gridAreas[index];
        public int AreaCount
        {
            get => Width * Height;
            set
            {
                int width = Math.Clamp(Math.CeilToInt(Math.Sqrt(value)), 1, _maxSize);
                if (Width != width)
                {
                    Width = Height = width;
                }
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly GridArea[] _gridAreas;

        private float _xBounds = 192;
        private float _yBounds = 108;

        private float _areaWidth;
        private float _areaHeight;

        public DynamicGrid()
        {
            Width = 1;
            Height = 1;
            _gridAreas = new GridArea[_maxSize * _maxSize];
            for (int i = 0; i < _gridAreas.Length; i++)
            {
                _gridAreas[i] = new GridArea(512);
            }
        }

        public void AddEntity(Transform transform, Collider collider, int id)
        {
            float xScale;
            float yScale;
            if (collider.colliderShape == ColliderShape.Circle)
            {
                xScale = yScale = collider.radius * transform.scale;
            }
            else
            {
                xScale = collider.width * transform.scale;
                yScale = collider.height * transform.scale;
            }
            int x = (int)((transform.x - xScale) / _areaWidth);
            int xMax = (int)((transform.x + xScale) / _areaWidth);

            int yMax = (int)((transform.y + yScale) / _areaHeight);
            int y = (int)((transform.y - yScale) / _areaHeight);

            if (x < 0) x = 0;
            if (y < 0) y = 0;

            for (int xx = x; xx <= xMax; xx++)
            {
                for (int yy = y; yy <= yMax; yy++)
                {
                    _gridAreas[yy * Width + xx].Add(id);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _gridAreas.Length; i++)
            {
                _gridAreas[i].Clear();
            }
        }

        public void SetBounds(Bounds bounds)
        {
            _xBounds = bounds.width;
            _yBounds = bounds.height;
            _areaWidth = _xBounds / Width;
            _areaHeight = _yBounds / Height;
        }
    }
}

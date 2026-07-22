using UnityEngine;

namespace Bas.Thoth
{
    public class DynamicGrid
    {
        private static readonly int _maxWidth = 400;
        private static readonly int _maxHeight = 400;

        public GridArea this[int index] => _gridAreas[index];
        public int AreaCount
        {
            get => _width * _height;
            set
            {
                _width = Mathf.Clamp(Mathf.CeilToInt(Mathf.Sqrt(value)), 1, _maxWidth);
                _height = (value / _width);
            }
        }

        public int _width;
        public int _height;

        private readonly GridArea[] _gridAreas;

        private float _xBounds = 192;
        private float _yBounds = 108;

        private float _areaWidth;
        private float _areaHeight;

        public DynamicGrid()
        {
            _width = 1;
            _height = 1;
            _gridAreas = new GridArea[_maxWidth * _maxHeight];
            for (int i = 0; i < _gridAreas.Length; i++)
            {
                _gridAreas[i] = new GridArea(512);
            }
        }

        public void AddEntity(Models.Transform transform, int id)
        {
            int x = (int)((transform.x + _xBounds - transform.scale) / _areaWidth);
            int xMax = (int)((transform.x + _xBounds + transform.scale) / _areaWidth);

            int yMax = (int)((transform.y + _yBounds + transform.scale) / _areaHeight);
            int y = (int)((transform.y + _yBounds - transform.scale) / _areaHeight);

            for (; x <= xMax; x++)
            {
                for (; y <= yMax; y++)
                {
                    _gridAreas[y * _width + x].Add(id);
                }
            }
        }

        public void Clear()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _gridAreas[x + y * _width].Clear();
                }
            }
        }

        public void SetBounds(Bounds bounds)
        {
            _xBounds = bounds.extents.x;
            _yBounds = bounds.extents.y;
            _areaWidth = _xBounds * 2 / _width;
            _areaHeight = _yBounds * 2 / _height;
        }
    }
}

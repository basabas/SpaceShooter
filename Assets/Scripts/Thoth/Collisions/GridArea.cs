namespace Bas.Thoth
{
    public class GridArea
    {
        public int this[int i]
        {
            get => _indexes[i];
            set => _indexes[i] = value;
        }

        public int Count { get; private set; }

        private readonly int[] _indexes;
        private readonly int _maxCount;

        public GridArea(int max)
        {
            Count = 0;
            _indexes = new int[max];
            _maxCount = max;
        }

        public void Add(int index)
        {
            if (Count < _maxCount)
            {
                _indexes[Count++] = index;
            }
            //else
            //{
            //    Debug.LogError($"Out of space: gridArea[{_index}] Count:{Count}, _maxCount:{_maxCount}");
            //}
        }

        public void RemoveAt(int i)
        {
            if (i < Count)
            {
                _indexes[i] = _indexes[Count--];
            }
        }

        public void Clear()
        {
            Count = 0;
        }
    }
}

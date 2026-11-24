using System.Collections.Generic;

namespace Bas.Thoth.Topdown.Factory
{
    public class PlayerInventory
    {
        private readonly Dictionary<int, int> _counts = new();

        public IReadOnlyDictionary<int, int> Counts => _counts;

        public int GetCount(int itemId)
        {
            if (itemId < 0)
            {
                return 0;
            }

            return _counts.TryGetValue(itemId, out int count) ? count : 0;
        }

        public void Add(int itemId, int amount)
        {
            if (itemId < 0 || amount <= 0)
            {
                return;
            }

            _counts.TryGetValue(itemId, out int current);
            _counts[itemId] = current + amount;
        }

        public bool TryRemove(int itemId, int amount)
        {
            if (itemId < 0 || amount <= 0)
            {
                return false;
            }

            if (!_counts.TryGetValue(itemId, out int current) || current < amount)
            {
                return false;
            }

            current -= amount;
            if (current <= 0)
            {
                _counts.Remove(itemId);
            }
            else
            {
                _counts[itemId] = current;
            }

            return true;
        }

        public bool Has(int itemId, int amount) => GetCount(itemId) >= amount;

        public void Clear() => _counts.Clear();
    }
}

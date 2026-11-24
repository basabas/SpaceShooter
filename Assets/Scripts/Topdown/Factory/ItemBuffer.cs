using System;

namespace Bas.Thoth.Topdown.Factory
{
    public class ItemBuffer
    {
        private readonly ItemStack[] _slots;

        public int SlotCount => _slots.Length;

        public ItemBuffer(int slotCount)
        {
            _slots = new ItemStack[System.Math.Max(1, slotCount)];
            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i] = ItemStack.Empty;
            }
        }

        public ItemStack Get(int index) => index >= 0 && index < _slots.Length ? _slots[index] : ItemStack.Empty;

        public void Set(int index, ItemStack stack)
        {
            if (index >= 0 && index < _slots.Length)
            {
                _slots[index] = stack.count <= 0 ? ItemStack.Empty : stack;
            }
        }

        public int CountOf(int itemId)
        {
            int total = 0;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].itemId == itemId)
                {
                    total += _slots[i].count;
                }
            }

            return total;
        }

        public bool TryInsert(int itemId, int amount, int maxStack, out int remaining)
        {
            remaining = amount;
            if (itemId < 0 || amount <= 0)
            {
                return false;
            }

            for (int i = 0; i < _slots.Length && remaining > 0; i++)
            {
                if (_slots[i].itemId == itemId && _slots[i].count < maxStack)
                {
                    int space = maxStack - _slots[i].count;
                    int moved = System.Math.Min(space, remaining);
                    _slots[i] = ItemStack.Of(itemId, _slots[i].count + moved);
                    remaining -= moved;
                }
            }

            for (int i = 0; i < _slots.Length && remaining > 0; i++)
            {
                if (_slots[i].IsEmpty)
                {
                    int moved = System.Math.Min(maxStack, remaining);
                    _slots[i] = ItemStack.Of(itemId, moved);
                    remaining -= moved;
                }
            }

            return remaining < amount;
        }

        public bool TryRemove(int itemId, int amount)
        {
            if (CountOf(itemId) < amount)
            {
                return false;
            }

            int remaining = amount;
            for (int i = 0; i < _slots.Length && remaining > 0; i++)
            {
                if (_slots[i].itemId != itemId)
                {
                    continue;
                }

                int take = System.Math.Min(_slots[i].count, remaining);
                int left = _slots[i].count - take;
                _slots[i] = left <= 0 ? ItemStack.Empty : ItemStack.Of(itemId, left);
                remaining -= take;
            }

            return true;
        }

        public bool TryTakeAny(out ItemStack stack)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (!_slots[i].IsEmpty)
                {
                    stack = ItemStack.Of(_slots[i].itemId, 1);
                    int left = _slots[i].count - 1;
                    _slots[i] = left <= 0 ? ItemStack.Empty : ItemStack.Of(_slots[i].itemId, left);
                    return true;
                }
            }

            stack = ItemStack.Empty;
            return false;
        }

        public ItemStack[] Snapshot()
        {
            ItemStack[] copy = new ItemStack[_slots.Length];
            Array.Copy(_slots, copy, _slots.Length);
            return copy;
        }
    }
}

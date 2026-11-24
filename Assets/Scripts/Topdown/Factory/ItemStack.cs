namespace Bas.Thoth.Topdown.Factory
{
    public struct ItemStack
    {
        public int itemId;
        public int count;

        public bool IsEmpty => count <= 0 || itemId < 0;

        public static ItemStack Empty => new ItemStack { itemId = -1, count = 0 };

        public static ItemStack Of(int itemId, int count) => new ItemStack { itemId = itemId, count = count };
    }
}

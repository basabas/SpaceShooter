using UnityEngine;

namespace Bas.Thoth.Topdown.Factory
{
    [CreateAssetMenu(fileName = "Item", menuName = "Factory/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public int maxStack = 100;
        public Color color = Color.white;

        public void CopyFrom(ItemDefinition other)
        {
            id = other.id;
            displayName = other.displayName;
            maxStack = other.maxStack;
            color = other.color;
        }
    }
}

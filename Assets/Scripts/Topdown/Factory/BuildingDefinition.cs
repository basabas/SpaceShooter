using UnityEngine;

namespace Bas.Thoth.Topdown.Factory
{
    [CreateAssetMenu(fileName = "Building", menuName = "Factory/Building Definition")]
    public class BuildingDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public BuildingKind kind;
        public string itemId;
        public float powerProduction;
        public float powerConsumption;
        public Color color = Color.white;
        public float scaleMultiplier = 0.9f;
        public bool isMachine;
        public int inputSlots = 4;
        public int outputSlots = 2;
    }
}

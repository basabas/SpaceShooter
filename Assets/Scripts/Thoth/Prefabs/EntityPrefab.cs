using UnityEngine;

namespace Bas.Thoth
{
    [CreateAssetMenu(fileName = "New Entity Prefab", menuName = "Thoth/EntityPrefab")]
    public class EntityPrefab : ScriptableObject
    {
        [SerializeField, HideInInspector] 
        private int id;
        [SerializeField, HideInInspector] 
        private SerializeableTypeDictionary types = new();
        [SerializeField, HideInInspector] 
        private GraphicsInformation graphicsInformation;

        public GraphicsInformation GraphicsInformation => graphicsInformation;
        public SerializeableTypeDictionary Types => types;

        public override string ToString() => $"[EntityPrefab] name:{name} types:{types}";
    }
}

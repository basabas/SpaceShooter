using System;
using UnityEngine;

namespace Bas.Thoth.Topdown.Factory
{
    [Serializable]
    public struct RecipeItemAmount
    {
        public string itemId;
        public int count;
    }

    [CreateAssetMenu(fileName = "Recipe", menuName = "Factory/Recipe Definition")]
    public class RecipeDefinition : ScriptableObject
    {
        public string id;
        public BuildingKind machine;
        public float craftTime = 1f;
        public RecipeItemAmount[] inputs = Array.Empty<RecipeItemAmount>();
        public RecipeItemAmount[] outputs = Array.Empty<RecipeItemAmount>();
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Bas.Thoth.Topdown.Factory
{
    [CreateAssetMenu(fileName = "FactoryCatalog", menuName = "Factory/Factory Catalog")]
    public class FactoryCatalog : ScriptableObject
    {
        public const float TileSize = 20f;

        [SerializeField] private List<ItemDefinition> items = new();
        [SerializeField] private List<RecipeDefinition> recipes = new();
        [SerializeField] private List<BuildingDefinition> buildings = new();

        private readonly Dictionary<string, int> _itemIndexById = new();
        private readonly Dictionary<string, int> _recipeIndexById = new();
        private readonly Dictionary<string, int> _buildingIndexById = new();
        private readonly Dictionary<BuildingKind, int> _buildingIndexByKind = new();
        public IReadOnlyList<ItemDefinition> Items => items;
        public IReadOnlyList<RecipeDefinition> Recipes => recipes;
        public IReadOnlyList<BuildingDefinition> Buildings => buildings;

        public static FactoryCatalog LoadOrCreate()
        {
            FactoryCatalog catalog = Resources.Load<FactoryCatalog>("Factory/FactoryCatalog");
            if (catalog == null)
            {
                catalog = CreateInstance<FactoryCatalog>();
                catalog.PopulateDefaults();
            }

            catalog.BuildLookups();
            return catalog;
        }

        public void BuildLookups()
        {
            _itemIndexById.Clear();
            _recipeIndexById.Clear();
            _buildingIndexById.Clear();
            _buildingIndexByKind.Clear();

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && !string.IsNullOrEmpty(items[i].id))
                {
                    _itemIndexById[items[i].id] = i;
                }
            }

            for (int i = 0; i < recipes.Count; i++)
            {
                if (recipes[i] != null && !string.IsNullOrEmpty(recipes[i].id))
                {
                    _recipeIndexById[recipes[i].id] = i;
                }
            }

            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i] == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(buildings[i].id))
                {
                    _buildingIndexById[buildings[i].id] = i;
                }

                _buildingIndexByKind[buildings[i].kind] = i;
            }
        }

        public bool TryGetItemIndex(string id, out int index) => _itemIndexById.TryGetValue(id, out index);

        public ItemDefinition GetItem(int index) => index >= 0 && index < items.Count ? items[index] : null;

        public ItemDefinition GetItem(string id) => TryGetItemIndex(id, out int index) ? items[index] : null;

        public bool TryGetBuildingIndex(string id, out int index) => _buildingIndexById.TryGetValue(id, out index);

        public BuildingDefinition GetBuilding(int index) => index >= 0 && index < buildings.Count ? buildings[index] : null;

        public BuildingDefinition GetBuilding(BuildingKind kind) =>
            _buildingIndexByKind.TryGetValue(kind, out int index) ? buildings[index] : null;

        public RecipeDefinition GetRecipe(int index) => index >= 0 && index < recipes.Count ? recipes[index] : null;

        public RecipeDefinition FindRecipe(BuildingKind machine, IList<ItemStack> inputs)
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                RecipeDefinition recipe = recipes[i];
                if (recipe == null || recipe.machine != machine)
                {
                    continue;
                }

                if (MatchesInputs(recipe, inputs))
                {
                    return recipe;
                }
            }

            return null;
        }

        public int ResolveItemId(string id) => TryGetItemIndex(id, out int index) ? index : -1;

        private bool MatchesInputs(RecipeDefinition recipe, IList<ItemStack> inputs)
        {
            if (recipe.inputs == null || recipe.inputs.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < recipe.inputs.Length; i++)
            {
                RecipeItemAmount need = recipe.inputs[i];
                int itemIndex = ResolveItemId(need.itemId);
                int have = 0;
                for (int s = 0; s < inputs.Count; s++)
                {
                    if (inputs[s].itemId == itemIndex)
                    {
                        have += inputs[s].count;
                    }
                }

                if (have < need.count)
                {
                    return false;
                }
            }

            return true;
        }

        public void PopulateDefaults()
        {
            items = new List<ItemDefinition>();
            recipes = new List<RecipeDefinition>();
            buildings = new List<BuildingDefinition>();

            AddItem("iron_ore", "Iron Ore", 100, new Color(0.55f, 0.35f, 0.25f));
            AddItem("copper_ore", "Copper Ore", 100, new Color(0.75f, 0.45f, 0.2f));
            AddItem("helium", "Helium", 100, new Color(0.7f, 0.85f, 1f));
            AddItem("ice_water", "Ice Water", 100, new Color(0.55f, 0.75f, 1f));
            AddItem("silicon", "Silicon", 100, new Color(0.7f, 0.7f, 0.85f));
            AddItem("carbon", "Carbon", 100, new Color(0.2f, 0.2f, 0.2f));
            AddItem("iron_plate", "Iron Plate", 100, new Color(0.75f, 0.75f, 0.8f));
            AddItem("copper_plate", "Copper Plate", 100, new Color(0.85f, 0.55f, 0.25f));
            AddItem("ammo", "Ammo", 200, new Color(0.9f, 0.8f, 0.2f));
            AddItem("solar_panel", "Solar Panel", 50, new Color(0.2f, 0.35f, 0.7f));
            AddItem("battery", "Battery", 50, new Color(0.2f, 0.7f, 0.3f));
            AddItem("engine", "Engine", 20, new Color(0.4f, 0.4f, 0.45f));
            AddItem("hull", "Hull Plate", 200, new Color(0.45f, 0.5f, 0.55f));
            AddItem("belt", "Transport Belt", 200, new Color(0.85f, 0.7f, 0.2f));
            AddItem("inserter", "Inserter", 100, new Color(0.85f, 0.45f, 0.15f));
            AddItem("container", "Container", 50, new Color(0.5f, 0.4f, 0.3f));
            AddItem("crusher", "Crusher", 20, new Color(0.55f, 0.35f, 0.35f));
            AddItem("smelter", "Smelter", 20, new Color(0.8f, 0.35f, 0.2f));
            AddItem("refiner", "Refiner", 20, new Color(0.35f, 0.55f, 0.7f));
            AddItem("assembler", "Assembler", 20, new Color(0.4f, 0.65f, 0.4f));
            AddItem("constructor", "Constructor", 20, new Color(0.55f, 0.45f, 0.7f));

            AddBuilding("hull", "Hull", BuildingKind.Hull, "hull", 0, 0, new Color(0.4f, 0.45f, 0.5f, 0.9f), 1f, false, 0, 0);
            AddBuilding("belt", "Belt", BuildingKind.Belt, "belt", 0, 0, new Color(0.9f, 0.75f, 0.2f), 0.7f, false, 0, 0);
            AddBuilding("inserter", "Inserter", BuildingKind.Inserter, "inserter", 0, 0.05f, new Color(0.9f, 0.5f, 0.15f), 0.6f, false, 0, 0);
            AddBuilding("container", "Container", BuildingKind.Container, "container", 0, 0, new Color(0.55f, 0.4f, 0.25f), 0.85f, false, 8, 0);
            AddBuilding("solar_panel", "Solar Panel", BuildingKind.SolarPanel, "solar_panel", 30f, 0, new Color(0.15f, 0.3f, 0.75f), 0.9f, false, 0, 0);
            AddBuilding("crusher", "Crusher", BuildingKind.Crusher, "crusher", 0, 15f, new Color(0.6f, 0.35f, 0.35f), 0.95f, true, 4, 2);
            AddBuilding("smelter", "Smelter", BuildingKind.Smelter, "smelter", 0, 25f, new Color(0.85f, 0.3f, 0.15f), 0.95f, true, 4, 2);
            AddBuilding("refiner", "Refiner", BuildingKind.Refiner, "refiner", 0, 20f, new Color(0.3f, 0.55f, 0.75f), 0.95f, true, 4, 2);
            AddBuilding("assembler", "Assembler", BuildingKind.Assembler, "assembler", 0, 30f, new Color(0.35f, 0.7f, 0.4f), 0.95f, true, 4, 2);
            AddBuilding("constructor", "Constructor", BuildingKind.Constructor, "constructor", 0, 35f, new Color(0.55f, 0.4f, 0.75f), 0.95f, true, 4, 2);

            AddRecipe("iron_plate", BuildingKind.Smelter, 1.5f,
                new[] { Amount("iron_ore", 1) },
                new[] { Amount("iron_plate", 1) });
            AddRecipe("copper_plate", BuildingKind.Smelter, 1.5f,
                new[] { Amount("copper_ore", 1) },
                new[] { Amount("copper_plate", 1) });
            AddRecipe("crushed_silicon", BuildingKind.Crusher, 1f,
                new[] { Amount("silicon", 2) },
                new[] { Amount("silicon", 2) });
            AddRecipe("ammo", BuildingKind.Assembler, 2f,
                new[] { Amount("iron_plate", 2), Amount("copper_plate", 1) },
                new[] { Amount("ammo", 10) });
            AddRecipe("solar_panel_craft", BuildingKind.Assembler, 3f,
                new[] { Amount("copper_plate", 2), Amount("silicon", 3), Amount("iron_plate", 1) },
                new[] { Amount("solar_panel", 1) });
            AddRecipe("battery", BuildingKind.Refiner, 2.5f,
                new[] { Amount("copper_plate", 1), Amount("iron_plate", 1), Amount("carbon", 2) },
                new[] { Amount("battery", 1) });
            AddRecipe("engine", BuildingKind.Constructor, 4f,
                new[] { Amount("iron_plate", 5), Amount("copper_plate", 2), Amount("carbon", 1) },
                new[] { Amount("engine", 1) });
            AddRecipe("refined_helium", BuildingKind.Refiner, 2f,
                new[] { Amount("helium", 2) },
                new[] { Amount("helium", 2) });
        }

        private void AddItem(string id, string displayName, int maxStack, Color color)
        {
            ItemDefinition item = CreateInstance<ItemDefinition>();
            item.id = id;
            item.displayName = displayName;
            item.maxStack = maxStack;
            item.color = color;
            items.Add(item);
        }

        private void AddBuilding(
            string id,
            string displayName,
            BuildingKind kind,
            string itemId,
            float powerProduction,
            float powerConsumption,
            Color color,
            float scaleMultiplier,
            bool isMachine,
            int inputSlots,
            int outputSlots)
        {
            BuildingDefinition building = CreateInstance<BuildingDefinition>();
            building.id = id;
            building.displayName = displayName;
            building.kind = kind;
            building.itemId = itemId;
            building.powerProduction = powerProduction;
            building.powerConsumption = powerConsumption;
            building.color = color;
            building.scaleMultiplier = scaleMultiplier;
            building.isMachine = isMachine;
            building.inputSlots = inputSlots;
            building.outputSlots = outputSlots;
            buildings.Add(building);
        }

        private void AddRecipe(string id, BuildingKind machine, float craftTime, RecipeItemAmount[] inputs, RecipeItemAmount[] outputs)
        {
            RecipeDefinition recipe = CreateInstance<RecipeDefinition>();
            recipe.id = id;
            recipe.machine = machine;
            recipe.craftTime = craftTime;
            recipe.inputs = inputs;
            recipe.outputs = outputs;
            recipes.Add(recipe);
        }

        private static RecipeItemAmount Amount(string itemId, int count) => new RecipeItemAmount { itemId = itemId, count = count };
    }
}

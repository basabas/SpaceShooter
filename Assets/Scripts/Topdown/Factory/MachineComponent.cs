using System.Collections.Generic;

namespace Bas.Thoth.Topdown.Factory
{
    public class MachineComponent : Component<MachineInfo>
    {
        private FactoryContext _context;
        private PowerNetwork _power;
        private BuildingInventories _inventories;
        private FactoryCatalog _catalog;
        private readonly List<ItemStack> _inputScratch = new();

        public override void Init()
        {
            _context = Singletons.Get<FactoryContext>();
            _power = _context.Power;
            _inventories = _context.Inventories;
            _catalog = _context.Catalog;
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            MachineInfo info = this[entityID];
            info.powered = _power.IsPowered(entityID);
            if (!info.powered)
            {
                this[entityID] = info;
                return;
            }

            ItemBuffer inputs = _inventories.GetInputs(entityID);
            ItemBuffer outputs = _inventories.GetOutputs(entityID);
            if (inputs == null || outputs == null)
            {
                this[entityID] = info;
                return;
            }

            _inputScratch.Clear();
            for (int i = 0; i < inputs.SlotCount; i++)
            {
                ItemStack stack = inputs.Get(i);
                if (!stack.IsEmpty)
                {
                    _inputScratch.Add(stack);
                }
            }

            RecipeDefinition recipe = _catalog.FindRecipe(info.kind, _inputScratch);
            if (recipe == null)
            {
                info.progress = 0f;
                info.activeRecipeIndex = -1;
                this[entityID] = info;
                return;
            }

            info.activeRecipeIndex = IndexOfRecipe(recipe);
            info.craftTime = recipe.craftTime;
            info.progress += deltaTime;

            if (info.progress >= recipe.craftTime)
            {
                if (CanOutput(recipe, outputs) && ConsumeInputs(recipe, inputs))
                {
                    ProduceOutputs(recipe, outputs);
                    info.progress = 0f;
                }
                else
                {
                    info.progress = recipe.craftTime;
                }
            }

            this[entityID] = info;
        }

        private int IndexOfRecipe(RecipeDefinition recipe)
        {
            for (int i = 0; i < _catalog.Recipes.Count; i++)
            {
                if (_catalog.Recipes[i] == recipe)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool CanOutput(RecipeDefinition recipe, ItemBuffer outputs)
        {
            for (int i = 0; i < recipe.outputs.Length; i++)
            {
                int itemId = _catalog.ResolveItemId(recipe.outputs[i].itemId);
                ItemDefinition item = _catalog.GetItem(itemId);
                if (item == null)
                {
                    return false;
                }

                // Simulate insert into a copy-like check: require empty space roughly
                int free = 0;
                for (int s = 0; s < outputs.SlotCount; s++)
                {
                    ItemStack stack = outputs.Get(s);
                    if (stack.IsEmpty)
                    {
                        free += item.maxStack;
                    }
                    else if (stack.itemId == itemId)
                    {
                        free += item.maxStack - stack.count;
                    }
                }

                if (free < recipe.outputs[i].count)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ConsumeInputs(RecipeDefinition recipe, ItemBuffer inputs)
        {
            for (int i = 0; i < recipe.inputs.Length; i++)
            {
                int itemId = _catalog.ResolveItemId(recipe.inputs[i].itemId);
                if (inputs.CountOf(itemId) < recipe.inputs[i].count)
                {
                    return false;
                }
            }

            for (int i = 0; i < recipe.inputs.Length; i++)
            {
                int itemId = _catalog.ResolveItemId(recipe.inputs[i].itemId);
                inputs.TryRemove(itemId, recipe.inputs[i].count);
            }

            return true;
        }

        private void ProduceOutputs(RecipeDefinition recipe, ItemBuffer outputs)
        {
            for (int i = 0; i < recipe.outputs.Length; i++)
            {
                int itemId = _catalog.ResolveItemId(recipe.outputs[i].itemId);
                ItemDefinition item = _catalog.GetItem(itemId);
                outputs.TryInsert(itemId, recipe.outputs[i].count, item.maxStack, out _);
            }
        }
    }
}

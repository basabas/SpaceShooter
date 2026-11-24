using System.Collections.Generic;

namespace Bas.Thoth.Topdown.Factory
{
    public class BuildingInventories
    {
        private readonly Dictionary<int, ItemBuffer> _inputs = new();
        private readonly Dictionary<int, ItemBuffer> _outputs = new();
        private readonly Dictionary<int, ItemBuffer> _containers = new();

        public ItemBuffer GetInputs(int entityId) => _inputs.TryGetValue(entityId, out ItemBuffer buffer) ? buffer : null;

        public ItemBuffer GetOutputs(int entityId) => _outputs.TryGetValue(entityId, out ItemBuffer buffer) ? buffer : null;

        public ItemBuffer GetContainer(int entityId) => _containers.TryGetValue(entityId, out ItemBuffer buffer) ? buffer : null;

        public void RegisterMachine(int entityId, int inputSlots, int outputSlots)
        {
            _inputs[entityId] = new ItemBuffer(inputSlots);
            _outputs[entityId] = new ItemBuffer(outputSlots);
        }

        public void RegisterContainer(int entityId, int slots)
        {
            _containers[entityId] = new ItemBuffer(slots);
        }

        public void Unregister(int entityId)
        {
            _inputs.Remove(entityId);
            _outputs.Remove(entityId);
            _containers.Remove(entityId);
        }

        public bool TryInsert(int entityId, int itemId, int amount, int maxStack, bool preferInput)
        {
            if (preferInput && _inputs.TryGetValue(entityId, out ItemBuffer inputs))
            {
                inputs.TryInsert(itemId, amount, maxStack, out int remaining);
                return remaining < amount;
            }

            if (_containers.TryGetValue(entityId, out ItemBuffer container))
            {
                container.TryInsert(itemId, amount, maxStack, out int remaining);
                return remaining < amount;
            }

            if (_inputs.TryGetValue(entityId, out inputs))
            {
                inputs.TryInsert(itemId, amount, maxStack, out int remaining);
                return remaining < amount;
            }

            return false;
        }

        public bool TryExtract(int entityId, out ItemStack stack, bool preferOutput)
        {
            if (preferOutput && _outputs.TryGetValue(entityId, out ItemBuffer outputs) && outputs.TryTakeAny(out stack))
            {
                return true;
            }

            if (_containers.TryGetValue(entityId, out ItemBuffer container) && container.TryTakeAny(out stack))
            {
                return true;
            }

            if (_outputs.TryGetValue(entityId, out outputs) && outputs.TryTakeAny(out stack))
            {
                return true;
            }

            if (_inputs.TryGetValue(entityId, out ItemBuffer inputs) && inputs.TryTakeAny(out stack))
            {
                return true;
            }

            stack = ItemStack.Empty;
            return false;
        }
    }
}

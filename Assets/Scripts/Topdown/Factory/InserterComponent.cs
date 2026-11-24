namespace Bas.Thoth.Topdown.Factory
{
    public class InserterComponent : Component<InserterInfo>
    {
        private FactoryContext _context;
        private ShipGrid _grid;
        private GridPositionComponent _positions;
        private BuildingInventories _inventories;
        private BeltItemComponent _beltItems;
        private PowerNetwork _power;
        private MachineComponent _machines;
        private ContainerComponent _containers;

        public override void Init()
        {
            _context = Singletons.Get<FactoryContext>();
            _grid = _context.Grid;
            _positions = Singletons.Get<GridPositionComponent>();
            _inventories = _context.Inventories;
            _beltItems = Singletons.Get<BeltItemComponent>();
            _power = _context.Power;
            _machines = Singletons.Get<MachineComponent>();
            _containers = Singletons.Get<ContainerComponent>();
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            InserterInfo info = this[entityID];
            if (!_power.IsPowered(entityID))
            {
                this[entityID] = info;
                return;
            }

            info.timer -= deltaTime;
            if (info.timer > 0f)
            {
                this[entityID] = info;
                return;
            }

            if (!_positions.HasEntity(entityID))
            {
                return;
            }

            GridPosition pos = _positions[entityID];
            GridDirectionUtil.ToOffset(info.direction, out int fdx, out int fdy);
            int frontX = pos.x + fdx;
            int frontY = pos.y + fdy;
            int backX = pos.x - fdx;
            int backY = pos.y - fdy;

            if (info.heldCount <= 0)
            {
                if (TryPick(backX, backY, ref info))
                {
                    info.timer = info.cooldown;
                }
            }
            else if (TryDrop(frontX, frontY, ref info))
            {
                info.timer = info.cooldown;
            }

            this[entityID] = info;
        }

        private bool TryPick(int tileX, int tileY, ref InserterInfo info)
        {
            ShipTile tile = _grid.Get(tileX, tileY);
            if (!tile.HasBuilding)
            {
                return false;
            }

            int building = tile.buildingEntityId;

            if (_beltItems.TryExtractFromBelt(tileX, tileY, out ItemStack beltStack))
            {
                info.heldItemId = beltStack.itemId;
                info.heldCount = 1;
                return true;
            }

            bool preferOutput = _machines.HasEntity(building);
            if (_inventories.TryExtract(building, out ItemStack stack, preferOutput))
            {
                info.heldItemId = stack.itemId;
                info.heldCount = 1;
                return true;
            }

            return false;
        }

        private bool TryDrop(int tileX, int tileY, ref InserterInfo info)
        {
            ShipTile tile = _grid.Get(tileX, tileY);
            if (!tile.HasBuilding)
            {
                return false;
            }

            int building = tile.buildingEntityId;
            ItemDefinition item = _context.Catalog.GetItem(info.heldItemId);
            int maxStack = item != null ? item.maxStack : 100;

            if (_beltItems.TryInsertOntoBelt(tileX, tileY, info.heldItemId))
            {
                info.heldItemId = -1;
                info.heldCount = 0;
                return true;
            }

            bool preferInput = _machines.HasEntity(building) || _containers.HasEntity(building);
            if (_inventories.TryInsert(building, info.heldItemId, 1, maxStack, preferInput))
            {
                info.heldItemId = -1;
                info.heldCount = 0;
                return true;
            }

            return false;
        }
    }
}

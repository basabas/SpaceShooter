using Bas.Thoth.Models;
using Transform = Bas.Thoth.Models.Transform;

namespace Bas.Thoth.Topdown.Factory
{
    public class BuildingPlacer
    {
        private readonly FactoryContext _context;
        private readonly Entities _entities;
        private readonly TransformComponent _transforms;
        private readonly Graphics _graphics;
        private readonly GridPositionComponent _positions;
        private readonly BeltComponent _belts;
        private readonly InserterComponent _inserters;
        private readonly MachineComponent _machines;
        private readonly SolarComponent _solars;
        private readonly ContainerComponent _containers;
        private readonly BuildingInventories _inventories;

        public BuildingPlacer()
        {
            _context = Singletons.Get<FactoryContext>();
            _entities = Singletons.Get<Entities>();
            _transforms = Singletons.Get<TransformComponent>();
            _graphics = Singletons.Get<Graphics>();
            _positions = Singletons.Get<GridPositionComponent>();
            _belts = Singletons.Get<BeltComponent>();
            _inserters = Singletons.Get<InserterComponent>();
            _machines = Singletons.Get<MachineComponent>();
            _solars = Singletons.Get<SolarComponent>();
            _containers = Singletons.Get<ContainerComponent>();
            _inventories = _context.Inventories;
        }

        public bool TryPlaceHull(int tileX, int tileY, bool free)
        {
            ShipGrid grid = _context.Grid;
            ShipTile tile = grid.Get(tileX, tileY);
            if (tile.HasHull)
            {
                return false;
            }

            if (!free && HasAnyHull(grid) && !grid.IsAdjacentToHull(tileX, tileY))
            {
                return false;
            }

            if (!free)
            {
                BuildingDefinition def = _context.Catalog.GetBuilding(BuildingKind.Hull);
                int itemId = _context.Catalog.ResolveItemId(def.itemId);
                if (!_context.Inventory.TryRemove(itemId, 1))
                {
                    return false;
                }
            }

            int hullId = CreateVisualEntity(BuildingKind.Hull, tileX, tileY, _context.BuildDirection);
            tile.hullEntityId = hullId;
            grid.Set(tileX, tileY, tile);
            return true;
        }

        public bool TryPlaceBuilding(BuildingKind kind, int tileX, int tileY, GridDirection direction, bool free)
        {
            if (kind == BuildingKind.Hull)
            {
                return TryPlaceHull(tileX, tileY, free);
            }

            ShipGrid grid = _context.Grid;
            ShipTile tile = grid.Get(tileX, tileY);
            if (!tile.HasHull || tile.HasBuilding)
            {
                return false;
            }

            BuildingDefinition def = _context.Catalog.GetBuilding(kind);
            if (def == null)
            {
                return false;
            }

            int itemId = _context.Catalog.ResolveItemId(def.itemId);
            if (!free && !_context.Inventory.TryRemove(itemId, 1))
            {
                return false;
            }

            int entityId = CreateVisualEntity(kind, tileX, tileY, direction);
            AttachBehaviour(kind, entityId, def, direction);

            tile.buildingEntityId = entityId;
            grid.Set(tileX, tileY, tile);
            return true;
        }

        public bool TryRemoveAt(int tileX, int tileY)
        {
            ShipGrid grid = _context.Grid;
            ShipTile tile = grid.Get(tileX, tileY);
            if (!tile.HasHull && !tile.HasBuilding)
            {
                return false;
            }

            if (tile.HasBuilding)
            {
                RefundAndDestroyBuilding(tile.buildingEntityId);
                tile.buildingEntityId = -1;
            }
            else if (tile.HasHull)
            {
                // only remove hull if no building
                RefundHull();
                DestroyEntityFull(tile.hullEntityId);
                tile.hullEntityId = -1;
            }

            grid.Set(tileX, tileY, tile);
            return true;
        }

        private void RefundHull()
        {
            BuildingDefinition def = _context.Catalog.GetBuilding(BuildingKind.Hull);
            _context.Inventory.Add(_context.Catalog.ResolveItemId(def.itemId), 1);
        }

        private void RefundAndDestroyBuilding(int entityId)
        {
            if (_positions.HasEntity(entityId))
            {
                BuildingKind kind = _positions[entityId].kind;
                BuildingDefinition def = _context.Catalog.GetBuilding(kind);
                if (def != null)
                {
                    _context.Inventory.Add(_context.Catalog.ResolveItemId(def.itemId), 1);
                }
            }

            _inventories.Unregister(entityId);
            DestroyEntityFull(entityId);
        }

        private void DestroyEntityFull(int entityId)
        {
            if (entityId < 0)
            {
                return;
            }

            _entities.DestroyEntity(entityId);
        }

        private int CreateVisualEntity(BuildingKind kind, int tileX, int tileY, GridDirection direction)
        {
            BuildingDefinition def = _context.Catalog.GetBuilding(kind);
            int id = _entities.CreateEntity();

            _transforms.AddEntity(id);
            _graphics.AddEntity(id);
            _positions.AddEntity(id);

            Transform transform = _transforms[id];
            transform.scale = _context.Grid.TileSize * (def != null ? def.scaleMultiplier : 0.9f);
            transform.rotation = GridDirectionUtil.ToRotationDegrees(direction);
            _transforms[id] = transform;

            DrawInformation draw = _graphics[id];
            draw.textureIndex = _context.PlaceholderTextureIndex;
            draw.color = def != null ? def.color : UnityEngine.Color.white;
            _graphics[id] = draw;

            int defIndex = 0;
            for (int i = 0; i < _context.Catalog.Buildings.Count; i++)
            {
                if (_context.Catalog.Buildings[i] != null && _context.Catalog.Buildings[i].kind == kind)
                {
                    defIndex = i;
                    break;
                }
            }

            _positions[id] = new GridPosition
            {
                x = tileX,
                y = tileY,
                direction = direction,
                kind = kind,
                buildingDefIndex = defIndex
            };

            // Immediate sync
            if (Singletons.Get<Player>().HasEntity(Player.EntityID))
            {
                Transform ship = _transforms[Player.EntityID];
                _context.Grid.TileToWorld(tileX, tileY, ship.x, ship.y, out float wx, out float wy);
                transform.x = wx;
                transform.y = wy;
                _transforms[id] = transform;
            }

            return id;
        }

        private static bool HasAnyHull(ShipGrid grid)
        {
            foreach (var _ in grid.Tiles)
            {
                return true;
            }

            return false;
        }

        private void AttachBehaviour(BuildingKind kind, int entityId, BuildingDefinition def, GridDirection direction)
        {
            switch (kind)
            {
                case BuildingKind.Belt:
                    _belts.AddEntity(entityId);
                    _belts[entityId] = new BeltInfo { direction = direction, speed = 1.5f };
                    break;
                case BuildingKind.Inserter:
                    _inserters.AddEntity(entityId);
                    _inserters[entityId] = new InserterInfo
                    {
                        direction = direction,
                        cooldown = 0.35f,
                        timer = 0f,
                        heldItemId = -1,
                        heldCount = 0
                    };
                    break;
                case BuildingKind.Container:
                    _containers.AddEntity(entityId);
                    _containers[entityId] = new ContainerInfo();
                    _inventories.RegisterContainer(entityId, def.inputSlots > 0 ? def.inputSlots : 8);
                    break;
                case BuildingKind.SolarPanel:
                    _solars.AddEntity(entityId);
                    _solars[entityId] = new SolarInfo { production = def.powerProduction };
                    break;
                case BuildingKind.Crusher:
                case BuildingKind.Smelter:
                case BuildingKind.Refiner:
                case BuildingKind.Assembler:
                case BuildingKind.Constructor:
                    _machines.AddEntity(entityId);
                    _machines[entityId] = new MachineInfo
                    {
                        kind = kind,
                        progress = 0f,
                        craftTime = 1f,
                        activeRecipeIndex = -1,
                        powered = false
                    };
                    _inventories.RegisterMachine(entityId, def.inputSlots, def.outputSlots);
                    break;
            }
        }
    }
}

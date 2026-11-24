using Bas.Thoth.Models;
using System.Collections.Generic;
using Transform = Bas.Thoth.Models.Transform;

namespace Bas.Thoth.Topdown.Factory
{
    public class BeltItemComponent : Component<BeltItemInfo>
    {
        private FactoryContext _context;
        private ShipGrid _grid;
        private TransformComponent _transforms;
        private Graphics _graphics;
        private BeltComponent _belts;
        private Entities _entities;
        private readonly Dictionary<long, int> _itemByTile = new();

        public override void Init()
        {
            _context = Singletons.Get<FactoryContext>();
            _grid = _context.Grid;
            _transforms = Singletons.Get<TransformComponent>();
            _graphics = Singletons.Get<Graphics>();
            _belts = Singletons.Get<BeltComponent>();
            _entities = Singletons.Get<Entities>();
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            if (!Singletons.Get<Player>().HasEntity(Player.EntityID))
            {
                return;
            }

            BeltItemInfo item = this[entityID];
            long oldKey = ShipGrid.Key(item.tileX, item.tileY);
            ShipTile tile = _grid.Get(item.tileX, item.tileY);
            if (!tile.HasBuilding || !_belts.HasEntity(tile.buildingEntityId))
            {
                _entities.DestroyEntity(entityID);
                return;
            }

            BeltInfo belt = _belts[tile.buildingEntityId];
            item.progress += belt.speed * deltaTime;

            if (item.progress >= 1f)
            {
                GridDirectionUtil.ToOffset(belt.direction, out int dx, out int dy);
                int nextX = item.tileX + dx;
                int nextY = item.tileY + dy;
                long nextKey = ShipGrid.Key(nextX, nextY);
                ShipTile next = _grid.Get(nextX, nextY);

                if (next.HasBuilding && _belts.HasEntity(next.buildingEntityId) && !_itemByTile.ContainsKey(nextKey))
                {
                    _itemByTile.Remove(oldKey);
                    item.tileX = nextX;
                    item.tileY = nextY;
                    item.progress -= 1f;
                    _itemByTile[nextKey] = entityID;
                }
                else
                {
                    item.progress = 0.99f;
                }
            }

            this[entityID] = item;

            Transform ship = _transforms[Player.EntityID];
            _grid.TileToWorld(item.tileX, item.tileY, ship.x, ship.y, out float baseX, out float baseY);
            GridDirectionUtil.ToOffset(belt.direction, out int ox, out int oy);
            float offset = (item.progress - 0.5f) * _grid.TileSize;

            Transform transform = _transforms[entityID];
            transform.x = baseX + ox * offset;
            transform.y = baseY + oy * offset;
            transform.scale = _grid.TileSize * 0.35f;
            _transforms[entityID] = transform;
        }

        public bool TryInsertOntoBelt(int tileX, int tileY, int itemId)
        {
            long key = ShipGrid.Key(tileX, tileY);
            if (_itemByTile.ContainsKey(key))
            {
                return false;
            }

            ShipTile tile = _grid.Get(tileX, tileY);
            if (!tile.HasBuilding || !_belts.HasEntity(tile.buildingEntityId))
            {
                return false;
            }

            CreateItem(itemId, tileX, tileY);
            return true;
        }

        public bool TryExtractFromBelt(int tileX, int tileY, out ItemStack stack)
        {
            long key = ShipGrid.Key(tileX, tileY);
            if (!_itemByTile.TryGetValue(key, out int entityId) || !HasEntity(entityId))
            {
                stack = ItemStack.Empty;
                return false;
            }

            BeltItemInfo info = this[entityId];
            stack = ItemStack.Of(info.itemId, 1);
            RemoveEntity(entityId);
            _graphics.RemoveEntity(entityId);
            _transforms.RemoveEntity(entityId);
            _entities.DestroyEntity(entityId);
            return true;
        }

        private int CreateItem(int itemId, int tileX, int tileY)
        {
            int id = _entities.CreateEntity();
            AddEntity(id);
            _transforms.AddEntity(id);
            _graphics.AddEntity(id);

            ItemDefinition def = _context.Catalog.GetItem(itemId);
            DrawInformation draw = _graphics[id];
            draw.textureIndex = _context.PlaceholderTextureIndex;
            draw.color = def != null ? def.color : UnityEngine.Color.white;
            _graphics[id] = draw;

            this[id] = new BeltItemInfo
            {
                itemId = itemId,
                tileX = tileX,
                tileY = tileY,
                progress = 0.05f
            };

            _itemByTile[ShipGrid.Key(tileX, tileY)] = id;

            Transform t = _transforms[id];
            t.scale = _grid.TileSize * 0.35f;
            _transforms[id] = t;
            return id;
        }

        public override bool RemoveEntity(int entityID)
        {
            if (HasEntity(entityID))
            {
                BeltItemInfo info = this[entityID];
                long key = ShipGrid.Key(info.tileX, info.tileY);
                if (_itemByTile.TryGetValue(key, out int mapped) && mapped == entityID)
                {
                    _itemByTile.Remove(key);
                }
            }

            return base.RemoveEntity(entityID);
        }
    }
}

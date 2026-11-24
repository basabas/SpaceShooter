using Bas.Thoth.Models;
using Transform = Bas.Thoth.Models.Transform;

namespace Bas.Thoth.Topdown.Factory
{
    public class GridPositionComponent : Component<GridPosition>
    {
        private TransformComponent _transforms;
        private ShipGrid _grid;

        public override void Init()
        {
            _transforms = Singletons.Get<TransformComponent>();
            _grid = Singletons.Get<ShipGrid>();
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            if (Player.EntityID < 0)
            {
                return;
            }

            Transform ship = _transforms[Player.EntityID];
            GridPosition gridPos = this[entityID];
            _grid.TileToWorld(gridPos.x, gridPos.y, ship.x, ship.y, out float worldX, out float worldY);

            Transform transform = _transforms[entityID];
            transform.x = worldX;
            transform.y = worldY;

            if (gridPos.kind == BuildingKind.Belt || gridPos.kind == BuildingKind.Inserter)
            {
                transform.rotation = GridDirectionUtil.ToRotationDegrees(gridPos.direction);
            }

            _transforms[entityID] = transform;
        }
    }
}

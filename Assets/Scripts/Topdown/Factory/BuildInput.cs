using UnityEngine;
using UnityEngine.InputSystem;
using Transform = Bas.Thoth.Models.Transform;

namespace Bas.Thoth.Topdown.Factory
{
    public class BuildInput : Component
    {
        private FactoryContext _context;
        private TransformComponent _transforms;
        private BuildingPlacer _placer;
        private Camera _camera;
        private int _handItemIndex;

        private static readonly string[] HandItems =
        {
            "iron_ore", "copper_ore", "silicon", "carbon", "helium", "ice_water",
            "iron_plate", "copper_plate"
        };

        public override void Init()
        {
            _context = Singletons.Get<FactoryContext>();
            _transforms = Singletons.Get<TransformComponent>();
            _placer = Singletons.Get<BuildingPlacer>();
            _camera = Object.FindAnyObjectByType<Camera>();
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            if (keyboard == null || mouse == null || _camera == null)
            {
                return;
            }

            if (keyboard.digit1Key.wasPressedThisFrame) _context.SelectedHotbarIndex = 0;
            if (keyboard.digit2Key.wasPressedThisFrame) _context.SelectedHotbarIndex = 1;
            if (keyboard.digit3Key.wasPressedThisFrame) _context.SelectedHotbarIndex = 2;
            if (keyboard.digit4Key.wasPressedThisFrame) _context.SelectedHotbarIndex = 3;
            if (keyboard.digit5Key.wasPressedThisFrame) _context.SelectedHotbarIndex = 4;
            if (keyboard.digit6Key.wasPressedThisFrame) _context.SelectedHotbarIndex = 5;
            if (keyboard.digit7Key.wasPressedThisFrame) _context.SelectedHotbarIndex = 6;
            if (keyboard.digit8Key.wasPressedThisFrame) _context.SelectedHotbarIndex = 7;
            if (keyboard.digit9Key.wasPressedThisFrame) _context.SelectedHotbarIndex = 8;

            if (keyboard.rKey.wasPressedThisFrame)
            {
                _context.BuildDirection = GridDirectionUtil.RotateCW(_context.BuildDirection);
            }

            if (keyboard.tabKey.wasPressedThisFrame)
            {
                _handItemIndex = (_handItemIndex + 1) % HandItems.Length;
            }

            Transform ship = _transforms[Player.EntityID];
            Vector3 mouseWorld = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
            _context.Grid.WorldToTile(mouseWorld.x, mouseWorld.y, ship.x, ship.y, out int tileX, out int tileY);

            DrawGhost(tileX, tileY, ship.x, ship.y);

            if (mouse.leftButton.wasPressedThisFrame)
            {
                _placer.TryPlaceBuilding(_context.SelectedKind, tileX, tileY, _context.BuildDirection, free: false);
            }

            if (mouse.rightButton.wasPressedThisFrame)
            {
                _placer.TryRemoveAt(tileX, tileY);
            }

            if (keyboard.eKey.wasPressedThisFrame)
            {
                TryHandInsert(tileX, tileY);
            }

            if (keyboard.qKey.wasPressedThisFrame)
            {
                TryHandExtract(tileX, tileY);
            }
        }

        private void TryHandInsert(int tileX, int tileY)
        {
            ShipTile tile = _context.Grid.Get(tileX, tileY);
            if (!tile.HasBuilding)
            {
                return;
            }

            int itemId = _context.Catalog.ResolveItemId(HandItems[_handItemIndex]);
            if (!_context.Inventory.Has(itemId, 1))
            {
                return;
            }

            ItemDefinition item = _context.Catalog.GetItem(itemId);
            bool preferInput = true;
            if (_context.Inventories.TryInsert(tile.buildingEntityId, itemId, 1, item.maxStack, preferInput))
            {
                _context.Inventory.TryRemove(itemId, 1);
                return;
            }

            // belts
            if (Singletons.Get<BeltItemComponent>().TryInsertOntoBelt(tileX, tileY, itemId))
            {
                _context.Inventory.TryRemove(itemId, 1);
            }
        }

        private void TryHandExtract(int tileX, int tileY)
        {
            ShipTile tile = _context.Grid.Get(tileX, tileY);
            if (!tile.HasBuilding)
            {
                return;
            }

            if (Singletons.Get<BeltItemComponent>().TryExtractFromBelt(tileX, tileY, out ItemStack beltStack))
            {
                _context.Inventory.Add(beltStack.itemId, beltStack.count);
                return;
            }

            if (_context.Inventories.TryExtract(tile.buildingEntityId, out ItemStack stack, preferOutput: true))
            {
                _context.Inventory.Add(stack.itemId, stack.count);
            }
        }

        private void DrawGhost(int tileX, int tileY, float shipX, float shipY)
        {
            _context.Grid.TileToWorld(tileX, tileY, shipX, shipY, out float x, out float y);
            float half = _context.Grid.TileSize * 0.5f;
            Color color = _context.Grid.HasHull(tileX, tileY) ? Color.green : Color.yellow;
            if (_context.SelectedKind != BuildingKind.Hull && !_context.Grid.HasHull(tileX, tileY))
            {
                color = Color.red;
            }

            Vector3 a = new Vector3(x - half, y - half, 0);
            Vector3 b = new Vector3(x + half, y - half, 0);
            Vector3 c = new Vector3(x + half, y + half, 0);
            Vector3 d = new Vector3(x - half, y + half, 0);
            UnityEngine.Debug.DrawLine(a, b, color);
            UnityEngine.Debug.DrawLine(b, c, color);
            UnityEngine.Debug.DrawLine(c, d, color);
            UnityEngine.Debug.DrawLine(d, a, color);

            GridDirectionUtil.ToOffset(_context.BuildDirection, out int dx, out int dy);
            UnityEngine.Debug.DrawLine(new Vector3(x, y, 0), new Vector3(x + dx * half, y + dy * half, 0), Color.cyan);
        }

        public string HandItemId => HandItems[_handItemIndex];
    }
}

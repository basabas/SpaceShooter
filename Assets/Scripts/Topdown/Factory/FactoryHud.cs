using UnityEngine;

namespace Bas.Thoth.Topdown.Factory
{
    public class FactoryHud : MonoBehaviour
    {
        private FactoryContext _context;
        private BuildInput _buildInput;

        private void Awake()
        {
            _context = Singletons.Get<FactoryContext>();
            _buildInput = Singletons.Get<BuildInput>();
        }

        private void OnGUI()
        {
            if (_context?.Catalog == null)
            {
                return;
            }

            GUI.color = Color.white;
            float y = 10f;
            GUI.Label(new Rect(10, y, 900, 24), "Factory MVP  |  1-9 place  |  R rotate  |  LMB place  |  RMB remove  |  E insert  |  Q extract  |  Tab hand item");
            y += 22;

            BuildingKind selected = _context.SelectedKind;
            BuildingDefinition selectedDef = _context.Catalog.GetBuilding(selected);
            string selectedName = selectedDef != null ? selectedDef.displayName : selected.ToString();
            GUI.Label(new Rect(10, y, 900, 24), $"Selected: [{_context.SelectedHotbarIndex + 1}] {selectedName}  dir:{_context.BuildDirection}");
            y += 22;

            string handId = _buildInput != null ? _buildInput.HandItemId : "";
            ItemDefinition handItem = _context.Catalog.GetItem(handId);
            int handCount = handItem != null ? _context.Inventory.GetCount(_context.Catalog.ResolveItemId(handId)) : 0;
            string handName = handItem != null ? handItem.displayName : handId;
            GUI.Label(new Rect(10, y, 900, 24), $"Hand item: {handName} x{handCount}");
            y += 22;

            PowerNetwork power = _context.Power;
            GUI.color = power.Satisfied ? Color.green : Color.red;
            GUI.Label(new Rect(10, y, 900, 24), $"Power: {power.Production:0.0} / {power.Consumption:0.0}  ({(power.Satisfied ? "OK" : "STARVING")})");
            GUI.color = Color.white;
            y += 28;

            GUI.Label(new Rect(10, y, 900, 24), "Hotbar:");
            y += 20;
            for (int i = 0; i < _context.Hotbar.Length; i++)
            {
                BuildingDefinition def = _context.Catalog.GetBuilding(_context.Hotbar[i]);
                int itemId = def != null ? _context.Catalog.ResolveItemId(def.itemId) : -1;
                int count = _context.Inventory.GetCount(itemId);
                string mark = i == _context.SelectedHotbarIndex ? ">" : " ";
                string name = def != null ? def.displayName : _context.Hotbar[i].ToString();
                GUI.Label(new Rect(10, y, 500, 20), $"{mark}{i + 1}. {name}  ({count})");
                y += 18;
            }

            y += 8;
            GUI.Label(new Rect(10, y, 900, 24), "Inventory (resources):");
            y += 20;
            DrawCount("iron_ore", ref y);
            DrawCount("copper_ore", ref y);
            DrawCount("iron_plate", ref y);
            DrawCount("copper_plate", ref y);
            DrawCount("ammo", ref y);
            DrawCount("silicon", ref y);
            DrawCount("carbon", ref y);
        }

        private void DrawCount(string itemId, ref float y)
        {
            int id = _context.Catalog.ResolveItemId(itemId);
            ItemDefinition def = _context.Catalog.GetItem(id);
            if (def == null)
            {
                return;
            }

            GUI.Label(new Rect(20, y, 400, 18), $"{def.displayName}: {_context.Inventory.GetCount(id)}");
            y += 16;
        }
    }
}

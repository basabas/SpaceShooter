namespace Bas.Thoth.Topdown.Factory
{
    public class FactoryContext
    {
        public FactoryCatalog Catalog { get; set; }
        public ShipGrid Grid { get; set; }
        public PlayerInventory Inventory { get; set; }
        public BuildingInventories Inventories { get; set; }
        public PowerNetwork Power { get; set; }
        public int PlaceholderTextureIndex { get; set; }
        public int SelectedHotbarIndex { get; set; }
        public GridDirection BuildDirection { get; set; } = GridDirection.North;

        public BuildingKind[] Hotbar { get; } =
        {
            BuildingKind.Hull,
            BuildingKind.Belt,
            BuildingKind.Inserter,
            BuildingKind.Container,
            BuildingKind.SolarPanel,
            BuildingKind.Smelter,
            BuildingKind.Assembler,
            BuildingKind.Refiner,
            BuildingKind.Constructor
        };

        public BuildingKind SelectedKind => Hotbar[SelectedHotbarIndex];
    }
}

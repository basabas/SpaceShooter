using UnityEngine;

namespace Bas.Thoth.Topdown.Factory
{
    public static class FactoryGame
    {
        public static void Start(int playerEntityId)
        {
            FactoryContext context = Singletons.Get<FactoryContext>();
            context.Catalog = FactoryCatalog.LoadOrCreate();
            context.Grid = Singletons.Get<ShipGrid>();
            context.Grid.TileSize = FactoryCatalog.TileSize;
            context.Inventory = Singletons.Get<PlayerInventory>();
            context.Inventories = Singletons.Get<BuildingInventories>();
            context.Power = Singletons.Get<PowerNetwork>();
            context.PlaceholderTextureIndex = 0;
            context.SelectedHotbarIndex = 0;
            context.BuildDirection = GridDirection.East;

            // Register systems in the component update loop.
            // Power first so machines/inserters see current power this frame.
            Components components = Singletons.Get<Components>();
            int powerId = Singletons.Get<Entities>().CreateEntity();
            components.Get<PowerSystem>().AddEntity(powerId);
            components.Get<BuildInput>().AddEntity(playerEntityId);
            components.Get<GridPositionComponent>();
            components.Get<BeltComponent>();
            components.Get<BeltItemComponent>();
            components.Get<InserterComponent>();
            components.Get<MachineComponent>();
            components.Get<SolarComponent>();
            components.Get<ContainerComponent>();

            BuildingPlacer placer = Singletons.Get<BuildingPlacer>();

            GiveStarterInventory(context);
            SeedStarterShip(context, placer);

            if (Object.FindAnyObjectByType<FactoryHud>() == null)
            {
                new GameObject("FactoryHud", typeof(FactoryHud));
            }
        }

        private static void GiveStarterInventory(FactoryContext context)
        {
            PlayerInventory inv = context.Inventory;
            FactoryCatalog catalog = context.Catalog;

            void Add(string id, int count) => inv.Add(catalog.ResolveItemId(id), count);

            Add("hull", 200);
            Add("belt", 100);
            Add("inserter", 40);
            Add("container", 20);
            Add("solar_panel", 20);
            Add("crusher", 5);
            Add("smelter", 5);
            Add("refiner", 3);
            Add("assembler", 3);
            Add("constructor", 2);
            Add("iron_ore", 200);
            Add("copper_ore", 200);
            Add("silicon", 50);
            Add("carbon", 50);
            Add("helium", 30);
            Add("ice_water", 30);
        }

        private static void SeedStarterShip(FactoryContext context, BuildingPlacer placer)
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    placer.TryPlaceHull(x, y, free: true);
                }
            }

            placer.TryPlaceBuilding(BuildingKind.SolarPanel, -2, 2, GridDirection.North, free: true);
            placer.TryPlaceBuilding(BuildingKind.Container, 0, 0, GridDirection.North, free: true);

            ShipTile tile = context.Grid.Get(0, 0);
            if (tile.HasBuilding)
            {
                ItemBuffer container = context.Inventories.GetContainer(tile.buildingEntityId);
                if (container != null)
                {
                    int iron = context.Catalog.ResolveItemId("iron_ore");
                    ItemDefinition def = context.Catalog.GetItem(iron);
                    container.TryInsert(iron, 50, def.maxStack, out _);
                }
            }
        }
    }
}

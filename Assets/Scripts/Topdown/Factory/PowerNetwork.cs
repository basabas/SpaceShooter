using System.Collections.Generic;

namespace Bas.Thoth.Topdown.Factory
{
    public class PowerNetwork
    {
        public float Production { get; private set; }
        public float Consumption { get; private set; }
        public bool Satisfied => Production + 0.001f >= Consumption;

        private readonly HashSet<int> _poweredEntities = new();

        public bool IsPowered(int entityId) => Satisfied && _poweredEntities.Contains(entityId);

        public void Recalculate(
            ShipGrid grid,
            GridPositionComponent positions,
            SolarComponent solars,
            MachineComponent machines,
            InserterComponent inserters)
        {
            Production = 0f;
            Consumption = 0f;
            _poweredEntities.Clear();

            HashSet<long> visited = new();
            Queue<long> queue = new();

            foreach (KeyValuePair<long, ShipTile> pair in grid.Tiles)
            {
                if (!pair.Value.HasBuilding)
                {
                    continue;
                }

                int entityId = pair.Value.buildingEntityId;
                if (!solars.HasEntity(entityId))
                {
                    continue;
                }

                long key = pair.Key;
                if (visited.Add(key))
                {
                    queue.Enqueue(key);
                }
            }

            // If no solar, still mark nothing powered.
            while (queue.Count > 0)
            {
                long key = queue.Dequeue();
                ShipGrid.FromKey(key, out int x, out int y);
                ShipTile tile = grid.Get(x, y);
                if (!tile.HasHull)
                {
                    continue;
                }

                if (tile.HasBuilding)
                {
                    _poweredEntities.Add(tile.buildingEntityId);

                    if (solars.HasEntity(tile.buildingEntityId))
                    {
                        Production += solars[tile.buildingEntityId].production;
                    }

                    FactoryCatalog catalog = Singletons.Get<FactoryContext>().Catalog;

                    if (machines.HasEntity(tile.buildingEntityId))
                    {
                        BuildingDefinition def = catalog.GetBuilding(machines[tile.buildingEntityId].kind);
                        if (def != null)
                        {
                            Consumption += def.powerConsumption;
                        }
                    }

                    if (inserters.HasEntity(tile.buildingEntityId))
                    {
                        BuildingDefinition def = catalog.GetBuilding(BuildingKind.Inserter);
                        if (def != null)
                        {
                            Consumption += def.powerConsumption;
                        }
                    }
                }

                EnqueueNeighbor(visited, queue, grid, x + 1, y);
                EnqueueNeighbor(visited, queue, grid, x - 1, y);
                EnqueueNeighbor(visited, queue, grid, x, y + 1);
                EnqueueNeighbor(visited, queue, grid, x, y - 1);
            }
        }

        private static void EnqueueNeighbor(HashSet<long> visited, Queue<long> queue, ShipGrid grid, int x, int y)
        {
            if (!grid.HasHull(x, y))
            {
                return;
            }

            long key = ShipGrid.Key(x, y);
            if (visited.Add(key))
            {
                queue.Enqueue(key);
            }
        }
    }
}

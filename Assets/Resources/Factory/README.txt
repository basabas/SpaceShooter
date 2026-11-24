Factory data lives here.

At runtime, FactoryCatalog.LoadOrCreate() loads Resources/Factory/FactoryCatalog if present.
If missing, a full default catalog (items, buildings, recipes) is created in memory.

To customize:
1. In Unity, use Create > Factory > Item/Recipe/Building Definition
2. Create > Factory > Factory Catalog and assign the lists
3. Save the catalog asset as Resources/Factory/FactoryCatalog.asset

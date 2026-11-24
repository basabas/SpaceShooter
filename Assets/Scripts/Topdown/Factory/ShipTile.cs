namespace Bas.Thoth.Topdown.Factory
{
    public struct ShipTile
    {
        public int hullEntityId;
        public int buildingEntityId;

        public bool HasHull => hullEntityId >= 0;
        public bool HasBuilding => buildingEntityId >= 0;

        public static ShipTile Empty => new ShipTile { hullEntityId = -1, buildingEntityId = -1 };
    }
}

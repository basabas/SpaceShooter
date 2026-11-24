namespace Bas.Thoth.Topdown.Factory
{
    public enum GridDirection
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    public static class GridDirectionUtil
    {
        public static GridDirection RotateCW(GridDirection direction) => (GridDirection)(((int)direction + 1) % 4);

        public static void ToOffset(GridDirection direction, out int dx, out int dy)
        {
            switch (direction)
            {
                case GridDirection.North:
                    dx = 0;
                    dy = 1;
                    break;
                case GridDirection.East:
                    dx = 1;
                    dy = 0;
                    break;
                case GridDirection.South:
                    dx = 0;
                    dy = -1;
                    break;
                default:
                    dx = -1;
                    dy = 0;
                    break;
            }
        }

        public static float ToRotationDegrees(GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.North: return 0f;
                case GridDirection.East: return 90f;
                case GridDirection.South: return 180f;
                default: return 270f;
            }
        }
    }
}

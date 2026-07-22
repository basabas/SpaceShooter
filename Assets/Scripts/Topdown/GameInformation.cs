namespace Bas.Thoth.Topdown.Models
{
    public struct GameInformation
    {
        public float health;
        public float armor;

        public override string ToString() => $"Game[health:{health}, armor:{armor}]";
    }
}
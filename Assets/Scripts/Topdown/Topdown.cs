using System.Collections.Generic;

namespace Bas.Thoth.Topdown
{
    public class Topdown : ThothManager
    {
        private TopdownFactory _factory;

        protected override void PostStart()
        {
            _factory.CreatePlayer();
            _factory.CreateTestAnimatedThing(World.CenterX - 250, World.CenterY + 500, 100);
            CreateTestSpawners();
            _factory.CreateBackground();

            _factory.CreateBaseDefense(World.CenterX, -800);
        }

        private void CreateTestSpawners()
        {
            for (int i = 0; i < 100; i++)
            {
                _factory.CreateEnemySpawner(Random.Range(0, World.Width), Random.Range(0, World.Height), 1f, 10);
            }
        }

        protected override IEnumerable<Factory> GetFactories()
        {
            return new[] { _factory = Singletons.Get<TopdownFactory>() };
        }
    }
}

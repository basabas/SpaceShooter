using System.Collections.Generic;

namespace Bas.Thoth.Topdown
{
    public class Topdown : ThothManager
    {
        private TopdownFactory _factory;

        private int _playerId;

        protected override void PostStart()
        {
            _playerId = _factory.CreatePlayer();
            _factory.CreateTestAnimatedThing(50, 0, 100);
            CreateTestMissiles(20_000, 25);
            _factory.CreateBackground();
        }

        protected override IEnumerable<Factory> GetFactories()
        {
            return new[] { _factory = Singletons.Get<TopdownFactory>() };
        }

        private List<int> CreateTestMissiles(int amount, float scale)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < amount; i++)
            {
                float x = UnityEngine.Random.Range(-World.Width, World.Width);
                float y = UnityEngine.Random.Range(-World.Height, World.Height);
                x = UnityEngine.Random.Range(-World.CameraMaxWidth, World.CameraMaxWidth);
                y = UnityEngine.Random.Range(-World.CameraMaxWidth, World.CameraMaxWidth);
                result.Add(_factory.CreateMissile(x, y, _playerId, scale));
            }
            return result;
        }
    }
}

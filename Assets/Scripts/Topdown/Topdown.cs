using System.Collections.Generic;
using UnityEngine;

namespace Bas.Thoth.Topdown
{
    public class Topdown : ThothManager
    {
        private TopdownFactory _factory;

        protected override void PostStart()
        {
            _factory.CreateTestAnimatedThing(50, 0, 100);
            CreateTestMissiles(20_000, 25);
            _factory.CreateBackground();
            _factory.CreatePlayer();
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
                float rotation = 0;
                float vel = 0;
                float x = Random.Range(-World.Width, World.Width);
                float y = Random.Range(-World.Height, World.Height);
                x = Random.Range(-World.CameraMaxWidth, World.CameraMaxWidth);
                y = Random.Range(-World.CameraMaxWidth, World.CameraMaxWidth);
                float xVel = Mathf.Cos(rotation * Mathf.PI * Mathf.Deg2Rad) * vel;
                float yVel = Mathf.Sin(rotation * Mathf.PI * Mathf.Deg2Rad) * vel;
                result.Add(_factory.CreateMissile(x, y, rotation, xVel, yVel, scale));
            }
            return result;
        }
    }
}

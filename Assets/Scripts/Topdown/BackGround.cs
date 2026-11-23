using System.Collections.Generic;
using UnityEngine;

using Transform = Bas.Thoth.Models.Transform;

namespace Bas.Thoth.Topdown
{
    //TODO: magic numbers
    public class BackGround : Component
    {
        private const int _starAmount = 16_000;//8_000
        private const int _debrisAmount = 80;//20

        private const float _starMinSize = 1;
        private const float _starMaxSize = 2;
        private const float _starAlpha = .9f;

        private const float _debrisMinSize = 6;
        private const float _debrisMaxSize = 20;
        private const float _debrisAlpha = .05f;//0.05f

        private List<int> _stars = new List<int>();

        private RandomColorPicker _colorPicker;
        private TopdownFactory _factory;
        private TransformComponent _transformComponent;

        private float prevX;
        private float prevY;

        public override void Init()
        {
            _colorPicker = new RandomColorPicker("Art/Space2");
            _factory = Singletons.Get<TopdownFactory>();
            _transformComponent = Singletons.Get<TransformComponent>();

            float xColorOffset = Random.Range(0, .6f);
            float yColorOffset = Random.Range(0, .6f);

            for (int i = 0; i < _starAmount; i++)
            {
                float random = Math.Pow(Random.Range(0, 1f), 2);
                float scale = Math.Lerp(_starMinSize, _starMaxSize, random);

                float distributionX = Math.Lerp(World.CameraMaxWidth * .5f, World.CameraMaxWidth * .8f, random);
                float distributionY = Math.Lerp(World.CameraMaxWidth * .5f, World.CameraMaxWidth * .8f, random);

                float x = Random.Range(-distributionX, distributionX);
                float y = Random.Range(-distributionY, distributionY);

                float xx = (x + distributionX) / 5f;
                float yy = (y + distributionY) / 5f;

                Color color = _colorPicker.GetColor(xColorOffset + xx / distributionX, yColorOffset + yy / distributionY);
                color.a = _starAlpha;

                _stars.Add(_factory.CreateBackgroundStar(x, y, scale, color));
            }
            float debrisDistributionSize = World.CameraMaxWidth * .6f;
            for (int i = 0; i < _debrisAmount; i++)
            {
                float x = Random.Range(-debrisDistributionSize, debrisDistributionSize);
                float y = Random.Range(-debrisDistributionSize, debrisDistributionSize);
                float scale = Random.Range(_debrisMinSize, _debrisMaxSize);
                float rot = Random.Range(0, 360);
                float xVelocity = Random.Range(-scale, scale);
                float yVelocity = Random.Range(-scale, scale);
                float rotVelocity = Random.Range(-scale, scale) / 25f;
                Color color = new Color(1, 1, 1, _debrisAlpha);
                _factory.CreateSpaceDebris(x, y, scale, rot, xVelocity, yVelocity, rotVelocity, color);
            }

            //_factory.CreateAirPollution();
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            Transform playerTransform = _transformComponent[Player.EntityID];
            float x = playerTransform.x;
            float y = playerTransform.y;
            float xDif = x - prevX;
            float yDif = y - prevY;

            for (int i = 0; i < _stars.Count; i++)
            {
                Transform starTransform = _transformComponent[_stars[i]];
                float delta = Math.InverseLerp(_starMinSize, _starMaxSize, starTransform.scale);

                delta = Math.Lerp(.99f, .98f, delta);

                starTransform.x += xDif * delta;
                starTransform.y += yDif * delta;

                _transformComponent[_stars[i]] = starTransform;
            }

            prevX = x;
            prevY = y;
        }
    }
}
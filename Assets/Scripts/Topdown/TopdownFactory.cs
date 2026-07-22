using Bas.Thoth.Models;
using UnityEngine;
using Transform = Bas.Thoth.Models.Transform;

namespace Bas.Thoth.Topdown
{
    public class TopdownFactory : Factory
    {
        public int CreatePlayer()
        {
            return CreateEntity("Player");
        }

        public int CreateTestAnimatedThing(float x, float y, float scale)
        {
            int id = CreateEntity("Animated Thing");
            UpdateComponentValue<TransformComponent, Transform>(id, transform =>
            {
                transform.x = x;
                transform.y = y;
                transform.scale = scale;
                return transform;
            });
            return id;
        }

        public int CreateBackgroundStar(float x, float y, float scale, Color color)
        {
            int id = CreateEntity("BackgroundStar");

            UpdateComponentValue<TransformComponent, Transform>(id, transform =>
            {
                transform.x = x;
                transform.y = y;
                transform.scale = scale;
                return transform;
            });

            UpdateComponentValue<Graphics, DrawInformation>(id, drawInfo =>
            {
                drawInfo.color = color;
                return drawInfo;
            });
            return id;
        }

        public int CreateBackground() => CreateEntity(typeof(BackGround));

        public int CreateMissile(float x, float y, int targetId, float scale)
        {
            int id = CreateEntity("Missile");

            Transform target = Singletons.Get<TransformComponent>()[id];

            float dirX = target.x - x;
            float dirY = target.y - y;
            float rot = Math.GetAngle(dirX, dirY);

            UpdateComponentValue<TransformComponent, Transform>(id, transform =>
            {
                transform.x = x;
                transform.y = y;
                transform.scale = scale;
                transform.rotation = rot;
                return transform;
            });

            (float xNorm, float yNorm) = Math.GetNormalized(dirX, dirY);

            UpdateComponentValue<Physics, PhysicsInformation>(id, physics =>
            {
                float length = Math.GetLenght(physics.xVelocity, physics.yVelocity);
                physics.xVelocity = xNorm * length;
                physics.yVelocity = yNorm * length;
                return physics;
            });

            UpdateComponentValue<Missile, MissileInformation>(id, missile =>
            {
                missile.target = targetId;
                return missile;
            });
            return id;
        }

        public int CreateSpaceDebris(float x, float y, float scale, float rot, float xVelocity, float yVelocity, float rotVelocity, Color color)
        {
            int id = CreateEntity("SpaceDebris");
            UpdateComponentValue<TransformComponent, Transform>(id, transform =>
            {
                transform.x = x;
                transform.y = y;
                transform.scale = scale;
                transform.rotation = rot;
                return transform;
            });

            UpdateComponentValue<Physics, PhysicsInformation>(id, physics =>
            {
                physics.xVelocity = xVelocity;
                physics.yVelocity = yVelocity;
                physics.rotVelocity = rotVelocity;
                return physics;
            });

            UpdateComponentValue<Graphics, DrawInformation>(id, drawInfo =>
            {
                drawInfo.color = color;
                return drawInfo;
            });

            return id;
        }

        public int CreateAirPollution()
        {
            int id = CreateEntity(typeof(TransformComponent), typeof(Graphics));
            //_components.Get<TransformComponent>()[id] = new Models.Transform
            //{
            //    scale = 1000,
            //    rotation = 5
            //};
            //_components.Get<Graphics>()[id] = new DrawInformation
            //{
            //    entityType = EntityType.AirPolution,
            //    color = new Color(1, 1, 1, .3f)
            //};
            return id;
        }
    }
}

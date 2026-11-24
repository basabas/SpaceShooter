using Bas.Thoth.Models;
using UnityEngine;
using Transform = Bas.Thoth.Models.Transform;

namespace Bas.Thoth.Topdown
{
    public class TopdownFactory : Bas.Thoth.Factory
    {
        public int CreatePlayer()
        {
            int id = CreateEntity("Player");
            UpdateComponentValue<TransformComponent, Transform>(id, transform =>
            {
                transform.x = World.CenterX;
                transform.y = World.CenterY;
                transform.scale = 12f;
                return transform;
            });
            return id;
        }

        public int CreateTurret(int playerID, float x, float y)
        {
            int id = CreateEntity("Turret");

            Transform playerTransform = Singletons.Get<TransformComponent>()[playerID];

            UpdateComponentValue<RocketTurret, RocketTurretInformation>(id, turret =>
            {
                turret.x = x;
                turret.y = y;
                turret.fireTime = Random.Range(0f, 1f / turret.fireSpeed);
                return turret;
            });

            UpdateComponentValue<TransformComponent, Transform>(id, transform =>
            {
                transform.x = playerTransform.x + x;
                transform.y = playerTransform.y + y;
                transform.rotation = Math.GetAngle(x, y);
                return transform;
            });
            return id;
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
        public int CreateEnemySpawner(float x, float y, float rate, int max)
        {
            int id = CreateEntity(typeof(EnemySpawner));
            UpdateComponentValue<TransformComponent, Transform>(id, transform =>
            {
                transform.x = x;
                transform.y = y;
                return transform;
            });
            UpdateComponentValue<EnemySpawner, EnemySpawn>(id, spawnInfo =>
            {
                spawnInfo.x = x;
                spawnInfo.y = y;
                spawnInfo.rate = rate;
                spawnInfo.spawnTimer = Random.Range(0f, 1f / spawnInfo.rate);
                spawnInfo.max = max;
                return spawnInfo;
            });
            return id;
        }

        public int CreateMissile(float x, float y, int targetId, float scale, int damage)
        {
            int id = CreateEntity("Missile");

            Transform target = Singletons.Get<TransformComponent>()[targetId];

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
                missile.damage = damage;
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

        public int CreateBaseDefense(float x, float y)
        {
            int id = CreateEntity("BaseDefense");
            UpdateComponentValue<TransformComponent, Transform>(id, transform =>
            {
                transform.x = x;
                transform.y = y;
                return transform;
            });
            return id;
        }

        public int CreateEnemy(float x, float y)
        {
            int id = CreateEntity("Enemy");

            Transform target = Singletons.Get<TransformComponent>()[Player.EntityID];

            float dirX = target.x - x;
            float dirY = target.y - y;
            float rot = Math.GetAngle(dirX, dirY);

            UpdateComponentValue<TransformComponent, Transform>(id, transform =>
            {
                transform.x = x;
                transform.y = y;
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
            return id;
        }
    }
}

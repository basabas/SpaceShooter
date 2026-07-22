using Bas.Thoth.Models;
using System.Threading.Tasks;
using UnityEngine;
using Collider = Bas.Thoth.Models.Collider;
using Transform = Bas.Thoth.Models.Transform;

namespace Bas.Thoth
{
    public class Collision : Component<Collider>
    {
        private DynamicGrid _grid;

        private int _entitiesPerArea = 10;

        private int _currentAmount;

        private int _lastAmount;
        private float _intervalTimer;
        private int _sign = 1;

        private OverlapResolver _overlapResolver;
        private TransformComponent _transforms;

        private ParallelCollisionBuffer _collisionBuffer;
        private ParallelCollisionBuffer _overlapBuffer;

        private PhysicsResolver _physicsResolver;
        private OverlapHandler _triggerHandler;

        public override void Init()
        {
            _grid = new DynamicGrid();
            _overlapResolver = Singletons.Get<OverlapResolver>();
            _transforms = Singletons.Get<TransformComponent>();
            _collisionBuffer = new ParallelCollisionBuffer(128);
            _overlapBuffer = new ParallelCollisionBuffer(128);
            _physicsResolver = Singletons.Get<PhysicsResolver>();
            _triggerHandler = Singletons.Get<OverlapHandler>();
        }

        public override void Update(float deltaTime)
        {
            _intervalTimer += deltaTime;
            if (_intervalTimer > 15)
            {
                if (Mathf.Abs(_currentAmount - _lastAmount) > _currentAmount / 400)
                {
                    if (_currentAmount < _lastAmount)
                    {
                        _sign = -_sign;
                    }
                    _entitiesPerArea = Mathf.Clamp(_entitiesPerArea + _sign, 5, _currentAmount);
                }
                _lastAmount = _currentAmount;
                _intervalTimer = 0;
            }

            _grid.Clear();
            _grid.AreaCount = Mathf.Max(1, Count / Mathf.Clamp(_entitiesPerArea, 1, int.MaxValue));
            _grid.SetBounds(new Bounds(Vector3.zero, new Vector3(World.Width, World.Height) * 2));

            _collisionBuffer.Resize(_grid.AreaCount);
            _overlapBuffer.Resize(_grid.AreaCount);
            _collisionBuffer.Clear();
            _overlapBuffer.Clear();

            base.Update(deltaTime);
            Parallel.For(0, _grid.AreaCount, DoCollisionCheck);

            foreach (CollisionResult result in _collisionBuffer)
            {
                _physicsResolver.Resolve(result);
            }

            foreach (CollisionResult result in _overlapBuffer)
            {
                _triggerHandler.HandleTrigger(result);
            }
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            _overlapResolver.UpdateBounds(entityID);
            if (this[entityID].colliderType == ColliderType.None)
            {
                return;
            }
            _grid.AddEntity(_transforms[entityID], entityID);
        }

        private void DoCollisionCheck(int index)
        {
            GridArea gridArea = _grid[index];
            int gridAreaCount = gridArea.Count;
            CollisionResult collisionResult = new CollisionResult();
            for (int i = 0; i < gridAreaCount; i++)
            {
                int entity = gridArea[i];
                collisionResult.id = entity;
                Collider collider = this[entity];
                Transform transform = _transforms[entity];
                for (int j = i + 1; j < gridAreaCount; j++)
                {
                    int otherEntity = gridArea[j];
                    collisionResult.otherId = otherEntity;
                    Collider otherCollider = this[otherEntity];
                    if (!_overlapResolver.AabbOverlap(collider, otherCollider))
                    {
                        continue;
                    }

                    Transform otherTransform = _transforms[otherEntity];

                    if (!_overlapResolver.Overlaps(collider, otherCollider, transform, otherTransform, ref collisionResult))
                    {
                        continue;
                    }

                    if (collider.colliderType == ColliderType.Trigger || otherCollider.colliderType == ColliderType.Trigger)
                    {
                        _overlapBuffer.Add(index, collisionResult);
                    }
                    else
                    {
                        _collisionBuffer.Add(index, collisionResult);
                    }
                }
            }
        }
    }
}
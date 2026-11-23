using Bas.Thoth.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Collider = Bas.Thoth.Models.Collider;
using Transform = Bas.Thoth.Models.Transform;

namespace Bas.Thoth
{
    public class Collision : Component<Collider>
    {
        private DynamicGrid _grid;

        private int _entitiesPerArea = 10;

        private int _lastAmount;
        private float _intervalTimer;
        private int _sign = 1;

        private OverlapResolver _overlapResolver;
        private TransformComponent _transforms;

        private ParallelCollisionBuffer _collisionBuffer;
        private ParallelCollisionBuffer _overlapBuffer;

        private PhysicsResolver _physicsResolver;
        private OverlapHandler _overlapHandler;

        public override void Init()
        {
            _grid = new DynamicGrid();
            _overlapResolver = Singletons.Get<OverlapResolver>();
            _transforms = Singletons.Get<TransformComponent>();
            _collisionBuffer = new ParallelCollisionBuffer(128);
            _overlapBuffer = new ParallelCollisionBuffer(128);
            _physicsResolver = Singletons.Get<PhysicsResolver>();
            _overlapHandler = Singletons.Get<OverlapHandler>();
            _entitiesPerArea = Settings.EntitiesPerArea;
        }

        public override void Update(float deltaTime)
        {
            _intervalTimer += deltaTime;
            if (_intervalTimer > 15 && Settings.BalanceEntitiesPerArea)
            {
                if (Math.Abs(Count - _lastAmount) > Count / 400)
                {
                    if (Count < _lastAmount)
                    {
                        _sign = -_sign;
                    }
                    _entitiesPerArea = Math.Clamp(_entitiesPerArea + _sign, 5, Count);
                }
                _lastAmount = Count;
                _intervalTimer = 0;
            }

            _grid.AreaCount = Math.Max(1, Count / Math.Clamp(_entitiesPerArea, 1, int.MaxValue));
            _grid.SetBounds(new Bounds(World.Width, World.Height));

            _collisionBuffer.Resize(_grid.AreaCount);
            _overlapBuffer.Resize(_grid.AreaCount);
            _collisionBuffer.Clear();
            _overlapBuffer.Clear();

            base.Update(deltaTime);
            Parallel.For(0, _grid.AreaCount, DoCollisionCheck);

            List<CollisionResult>[] collisionResults = _collisionBuffer.results;
            for (int i = 0; i < collisionResults.Length; i++)
            {
                List<CollisionResult> list = collisionResults[i];
                for (int j = 0; j < list.Count; j++)
                {
                    _physicsResolver.Resolve(list[j]);
                }
            }

            List<CollisionResult>[] overlapResults = _overlapBuffer.results;
            for (int i = 0; i < overlapResults.Length; i++)
            {
                List<CollisionResult> list = overlapResults[i];
                for (int j = 0, n = list.Count; j < n; j++)
                {
                    _overlapHandler.HandleTrigger(list[j]);
                }
            }

            //OLD, more GC BS
            //foreach (CollisionResult result in _collisionBuffer)
            //{
            //    _physicsResolver.Resolve(result);
            //}

            //foreach (CollisionResult result in _overlapBuffer)
            //{
            //    _overlapHandler.HandleTrigger(result);
            //}

            _grid.Clear();

            if (Settings.DrawGrid)
            {
                DebugDrawGrid();
            }
        }

        private void DebugDrawGrid()
        {
            UnityEngine.Debug.DrawLine(UnityEngine.Vector3.zero, UnityEngine.Vector3.up * World.Height, UnityEngine.Color.red);
            UnityEngine.Debug.DrawLine(UnityEngine.Vector3.zero, UnityEngine.Vector3.right * World.Width, UnityEngine.Color.red);
            UnityEngine.Debug.DrawLine(UnityEngine.Vector3.up * World.Height, UnityEngine.Vector3.right * World.Width + UnityEngine.Vector3.up * World.Height, UnityEngine.Color.red);
            UnityEngine.Debug.DrawLine(UnityEngine.Vector3.right * World.Width, UnityEngine.Vector3.right * World.Width + UnityEngine.Vector3.up * World.Height, UnityEngine.Color.red);

            float areaWidth = World.Width / _grid.Width;
            float areaHeight = World.Height / _grid.Height;
            for (int y = 1; y < _grid.Height; y++)
            {
                UnityEngine.Vector3 height = UnityEngine.Vector3.up * areaHeight * y;
                UnityEngine.Debug.DrawLine(height, height + UnityEngine.Vector3.right * World.Width, UnityEngine.Color.green);
            }

            for (int x = 1; x < _grid.Width; x++)
            {
                UnityEngine.Vector3 width = UnityEngine.Vector3.right * areaWidth * x;
                UnityEngine.Debug.DrawLine(width, width + UnityEngine.Vector3.up * World.Height, UnityEngine.Color.green);
            }
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            Collider collider = this[entityID];
            if (collider.colliderType == ColliderType.None)
            {
                return;
            }
            _overlapResolver.UpdateBounds(entityID);
            _grid.AddEntity(_transforms[entityID], collider, entityID);
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
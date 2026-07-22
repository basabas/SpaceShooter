using Bas.Thoth.Models;
using Bas.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bas.Thoth
{
    public class Graphics : Component<DrawInformation>, IDisposable
    {
        private const string _atlasID = "atlas";
        private static readonly string _shaderName = "Unlit/InstancedUnlitTransparentColor";
        private static readonly string _framesID = "frames";

        private static readonly int _entityBufferID = Shader.PropertyToID("entityBuffer");
        private const int _entityStride = 4;
        private static readonly int _transformBufferID = Shader.PropertyToID("transformBuffer");
        private const int _transformStride = 16;
        private static readonly int _drawInfoBufferID = Shader.PropertyToID("drawInfoBuffer");
        private const int _drawInfoStride = 20;
        private static readonly int _frameInfoBufferID = Shader.PropertyToID("frameInfoBuffer");
        private const int _frameInfoStride = 20;

        private static readonly Bounds Bounds = new Bounds(Vector3.zero, Vector3.one * 100_000);

        private Material _material;
        private ComputeBuffer _entityBuffer;
        private ComputeBuffer _transformBuffer;
        private ComputeBuffer _drawInfoBuffer;
        private ComputeBuffer _frameInfoBuffer;
        private Mesh _mesh;

        private uint[] _args;
        private ComputeBuffer _argsBuffer;

        private TransformComponent _transforms;
        private List<int> newEntities = new List<int>();
        private SortedGraphicsEntities _sortedEntities;

        public override void Init()
        {
            _transforms = Singletons.Get<TransformComponent>();
        }

        public void SetGraphics(EntityPrefab[] entityPrefabs, Texture atlas)
        {
            _sortedEntities = new SortedGraphicsEntities(this, entityPrefabs.Length);
            _mesh = MeshHelper.GetQuadMesh(2, 2);

            _material = new Material(Shader.Find(_shaderName));
            _material.SetTexture(_atlasID, atlas);

            uint numIndices = _mesh ? _mesh.GetIndexCount(0) : 0;
            _args = new uint[] { numIndices, (uint)_sortedEntities.Count, 0, 0, 0 };
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            _argsBuffer.SetData(_args);

            _entityBuffer = new ComputeBuffer(_sortedEntities.Entities.Length, _entityStride);
            _material.SetBuffer(_entityBufferID, _entityBuffer);

            _transformBuffer = new ComputeBuffer(_transforms.Values.Length, _transformStride);
            _material.SetBuffer(_transformBufferID, _transformBuffer);

            _drawInfoBuffer = new ComputeBuffer(Values.Length, _drawInfoStride);
            _material.SetBuffer(_drawInfoBufferID, _drawInfoBuffer);

            FrameInformation[] frameInformation = CreateFrameInformation(entityPrefabs, out List<Vector4> frames);
            _frameInfoBuffer = new ComputeBuffer(frameInformation.Length, _frameInfoStride);
            _frameInfoBuffer.SetData(frameInformation);
            _material.SetBuffer(_frameInfoBufferID, _frameInfoBuffer);

            _material.SetVectorArray(_framesID, frames);
        }

        public override void Update(float deltaTime)
        {
            if (Count > 0)
            {
                newEntities.ForEach(entity => _sortedEntities.AddEntity(entity));
                newEntities.Clear();

                if (Count != _args[1])
                {
                    _args[1] = (uint)_sortedEntities.Count;
                    _argsBuffer.SetData(_args);
                }

                if (_entityBuffer.count != _sortedEntities.Entities.Length)
                {
                    _entityBuffer?.Release();
                    _entityBuffer = new ComputeBuffer(_sortedEntities.Entities.Length, _entityStride);
                    _material.SetBuffer(_entityBufferID, _entityBuffer);
                }
                if (_transformBuffer.count != _transforms.Values.Length)
                {
                    _transformBuffer?.Release();
                    _transformBuffer = new ComputeBuffer(_transforms.Values.Length, _transformStride);
                    _material.SetBuffer(_transformBufferID, _transformBuffer);
                }
                if (_drawInfoBuffer.count != Values.Length)
                {
                    _drawInfoBuffer?.Release();
                    _drawInfoBuffer = new ComputeBuffer(Values.Length, _drawInfoStride);
                    _material.SetBuffer(_drawInfoBufferID, _drawInfoBuffer);
                }

                _entityBuffer.SetData(_sortedEntities.Entities, 0, 0, _sortedEntities.Count);
                _transformBuffer.SetData(_transforms.Values, 0, 0, _transforms.Values.Length);
                _drawInfoBuffer.SetData(Values, 0, 0, Values.Length);
                UnityEngine.Graphics.DrawMeshInstancedIndirect(_mesh, 0, _material, Bounds, _argsBuffer);
            }
        }

        public override void UpdateEntity(int entityID, float deltaTime)
        {
            //We dont want to do anything with UpdateEntity
        }

        public override void AddEntity(int entityID)
        {
            newEntities.Add(entityID);
            base.AddEntity(entityID);
        }

        public override bool RemoveEntity(int entityID)
        {
            _sortedEntities.RemoveEntity(entityID);
            return base.RemoveEntity(entityID);
        }

        void IDisposable.Dispose()
        {
            _entityBuffer?.Release();
            _argsBuffer?.Release();
            _transformBuffer?.Release();
            _drawInfoBuffer?.Release();
            _frameInfoBuffer?.Release();
        }

        private FrameInformation[] CreateFrameInformation(EntityPrefab[] entityPrefabs, out List<Vector4> frames)
        {
            FrameInformation[] array = new FrameInformation[entityPrefabs.Length];
            frames = new List<Vector4>();

            for (int i = 0; i < entityPrefabs.Length; i++)
            {
                GraphicsInformation info = entityPrefabs[i].GraphicsInformation;
                array[i] = new FrameInformation
                {
                    fps = info.fps,
                    frameStart = frames.Count,
                    frameCount = info.rects.Count,
                    width = info.width,
                    height = info.height
                };
                frames.AddRange(info.rects);
            }

            return array;
        }
    }
}
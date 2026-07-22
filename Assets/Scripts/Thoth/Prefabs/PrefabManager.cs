using Bas.Thoth.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Bas.Thoth
{
    [CreateAssetMenu(fileName = "PrefabManager", menuName = "Thoth/PrefabManager")]
    public class PrefabManager : ScriptableObject
    {
        [SerializeField] private EntityPrefab[] prefabs;
        public bool InitializePrefabs(out EntityPrefab[] entityPrefabs, out Texture2D atlas)
        {
            atlas = new Texture2D(8192, 8192, TextureFormat.DXT5, false, false);
            List<Texture2D> textures = new List<Texture2D>();
            foreach (EntityPrefab entity in prefabs)
            {
                textures.AddRange(entity.GraphicsInformation.frames);
            }

            Rect[] rects = atlas.PackTextures(textures.ToArray(), 4, 8192);

            int frameIndex = 0;
            for (int i = 0; i < prefabs.Length; i++)
            {
                EntityPrefab entity = prefabs[i];
                entity.GraphicsInformation.rects.Clear();

                if (entity.GraphicsInformation.frames.Count == 0)
                {
                    continue;
                }

                for (int j = 0; j < entity.GraphicsInformation.frames.Count; j++)
                {
                    Vector4 vec4 = ToVector4(rects[j + frameIndex]);
                    entity.GraphicsInformation.rects.Add(vec4);
                }

                if (entity.Types.TryGetValue(typeof(Graphics), out AnyThing anything))
                {
                    DrawInformation drawInformation = anything.As<DrawInformation>();
                    drawInformation.textureIndex = i;
                    anything.Set(drawInformation);
                }

                frameIndex += entity.GraphicsInformation.frames.Count;
            }
            entityPrefabs = prefabs;
            return true;
        }

        public Texture2D GetAtlas(EntityPrefab[] entityPrefabs)
        {
            Texture2D atlas = new Texture2D(8192, 8192, TextureFormat.DXT5, false, false);
            List<Texture2D> textures = new List<Texture2D>();
            foreach (EntityPrefab entity in entityPrefabs)
            {
                textures.AddRange(entity.GraphicsInformation.frames);
            }

            Rect[] rects = atlas.PackTextures(textures.ToArray(), 4, 8192);

            int frameIndex = 0;
            for (int i = 0; i < entityPrefabs.Length; i++)
            {
                EntityPrefab entity = entityPrefabs[i];
                entity.GraphicsInformation.rects.Clear();

                if (entity.GraphicsInformation.frames.Count == 0)
                {
                    continue;
                }

                for (int j = 0; j < entity.GraphicsInformation.frames.Count; j++)
                {
                    Vector4 vec4 = ToVector4(rects[j + frameIndex]);
                    entity.GraphicsInformation.rects.Add(vec4);
                }

                if (entity.Types.TryGetValue(typeof(Graphics), out AnyThing anything))
                {
                    DrawInformation drawInformation = anything.As<DrawInformation>();
                    drawInformation.textureIndex = i;
                    anything.Set(drawInformation);
                }

                frameIndex += entity.GraphicsInformation.frames.Count;
            }
            return atlas;
        }

        private Vector4 ToVector4(Rect rect) => new Vector4(rect.x, rect.y, rect.x + rect.width, rect.y + rect.height);
    }
}

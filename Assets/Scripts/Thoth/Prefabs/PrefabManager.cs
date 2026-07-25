using Bas.Thoth.Models;
using Bas.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
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
                textures.AddRange(entity.GraphicsInformation.frames.Select(TextureFromSprite));
            }

            Rect[] rects = atlas.PackTextures(textures.ToArray(), 4, 8192);

            //TODO: Destroy textures of sprites that are 
            textures.Where(tex => tex.name == "Destroy me please").ForEach(Destroy);

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

        private Vector4 ToVector4(Rect rect) => new Vector4(rect.x, rect.y, rect.x + rect.width, rect.y + rect.height);
        public Texture2D TextureFromSprite(Sprite sprite)
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                int width = (int)sprite.rect.width;
                int height = (int)sprite.rect.height;
                Texture2D newTex = new Texture2D(width, height);
                newTex.name = "Destroy me please";
                Color[] defaultPixels = Enumerable.Repeat(Color.clear, width * height).ToArray();
                Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x
                , (int)sprite.textureRect.y
                , (int)sprite.textureRect.width
                , (int)sprite.textureRect.height);
                newTex.SetPixels(defaultPixels);
                newTex.SetPixels((int)sprite.textureRectOffset.x, (int)sprite.textureRectOffset.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height, pixels);
                newTex.Apply();
                return newTex;
            }
            else
            {
                return sprite.texture;
            }
        }
    }
}

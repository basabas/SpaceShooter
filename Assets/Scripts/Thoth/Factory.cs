using System;
using System.Collections.Generic;
using System.Linq;

namespace Bas.Thoth
{
    public abstract class Factory
    {
        private readonly Entities _entities;
        private readonly Components _components;

        Dictionary<string, EntityPrefab> prefabs;

        public Factory()
        {
            _entities = Singletons.Get<Entities>();
            _components = Singletons.Get<Components>();
        }

        public void SetPrefabs(EntityPrefab[] prefabs) => this.prefabs = prefabs.ToDictionary(prefab => prefab.name, prefab => prefab);

        public int CreateEntity(params Type[] componentTypes)
        {
            if (componentTypes.Length == 0)
            {
                UnityEngine.Debug.LogError("There is no reason to create an entity without components...");
            }
            int entityID = _entities.CreateEntity();

            foreach (Type t in componentTypes)
            {
                _components.Get(t).AddEntity(entityID);
            }

            return entityID;
        }

        public int CreateEntity(EntityPrefab prefab)
        {
            if (prefab.Types.Count == 0)
            {
                UnityEngine.Debug.LogError("There is no reason to create an entity without components...");
            }
            int entityID = _entities.CreateEntity();

            foreach ((Type type, AnyThing anything) in prefab.Types)
            {
                Component component = _components.Get(type);
                component.AddEntity(entityID);

                if (component is ICanSetAnything canSetAnything)
                {
                    canSetAnything.SetValue(entityID, anything);
                }
            }

            return entityID;
        }

        public int CreateEntity(string prefabName)
        {
            if (prefabs.TryGetValue(prefabName, out EntityPrefab prefab))
            {
                return CreateEntity(prefab);
            }
            throw new Exception($"Could not find Prefab with name \'{prefabName}\n, are you sure you spelled it right?");
        }

        public void UpdateComponentValue<T, TU>(int id, Func<TU, TU> updateFunction) where T : Component<TU> where TU : struct
        {
            _components.Get<T>().UpdateValue(id, updateFunction);
        }
    }
}

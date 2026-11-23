using Bas.Debug;
using System.Collections.Generic;
using UnityEngine;

namespace Bas.Thoth
{
    public abstract class ThothManager : MonoBehaviour
    {
        private Components _components;
        private Entities _entities;

        private List<Factory> _factoryies;

        private EntityPrefab[] prefabs;
        public Texture2D atlas;

        public void StartGame()
        {
            Application.targetFrameRate = Settings.FPS;

            _components = Singletons.Get<Components>();
            _entities = Singletons.Get<Entities>();
            _factoryies = new List<Factory>();

            PrefabManager prefabManager = Resources.Load<PrefabManager>("PrefabManager");
            if (!prefabManager.InitializePrefabs(out prefabs, out atlas))
            {
                Logging.LogError("YEAH. Nothing worked haha");
                return;
            }

            _factoryies.AddRange(GetFactories());

            _factoryies.ForEach(factory => factory.SetPrefabs(prefabs));
            _components.Get<Graphics>().SetGraphics(prefabs, atlas);

            PostStart();
        }

        protected abstract IEnumerable<Factory> GetFactories();

        protected virtual void PostStart()
        {
        }

        protected virtual void Update()
        {
            _components.Update(Time.deltaTime);
            _components.FlushDeadEntities(_entities.RemoveList);
            _entities.FlushDeadEntities();
        }

        protected virtual void OnDestroy()
        {
            _components.Dispose();
            if (atlas)
            {
                Destroy(atlas);
            }
        }
    }
}

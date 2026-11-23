using System;
using System.Collections.Generic;

namespace Bas.Thoth
{

    public class Components
    {
        private static readonly Type _componentType = typeof(Component);

        private Dictionary<Type, Component> _components = new();
        private Dictionary<Type, Component> _newComponents = new();

        public Component Get(Type type)
        {
            if (!_componentType.IsAssignableFrom(type))
            {
                throw new NotSupportedException($"Type {type.FullName} is not assignable from {_componentType.FullName}");
            }

            if (_components.TryGetValue(type, out Component component))
            {
                return component;
            }

            if (_newComponents.TryGetValue(type, out component))
            {
                return component;
            }

            component = (Component)Singletons.Get(type);
            _newComponents[type] = component;
            return component;
        }

        public T Get<T>() where T : Component => (T)Get(typeof(T));

        public void Update(float deltaTime)
        {
            foreach (var (type, component) in _newComponents)
            {
                _components[type] = component;
            }
            _newComponents.Clear();
            foreach (Component component in _components.Values)
            {
                component.Update(deltaTime);
            }
        }

        //TODO: Superslow and not efficient.
        public void FlushDeadEntities(IEnumerable<int> removeList)
        {
            foreach (Component component in _components.Values)
            {
                foreach (int entityID in removeList)
                {
                    component.RemoveEntity(entityID);
                }
            }
        }

        public void GetComponents(int entityID, List<Component> components)
        {
            foreach (Component component in _components.Values)
            {
                if (component.HasEntity(entityID))
                {
                    components.Add(component);
                }
            }
        }

        public void Dispose()
        {
            foreach (Component component in _components.Values)
            {
                if (component is IDisposable dispose)
                {
                    dispose.Dispose();
                }
            }
        }
    }
}
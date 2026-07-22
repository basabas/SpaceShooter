using System;
using System.Collections.Generic;

namespace Bas.Thoth
{

    public class Components
    {
        private static readonly Type _componentType = typeof(Component);

        private Dictionary<Type, Component> _components = new();

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

            component = (Component)Singletons.Get(type);
            _components.Add(type, component);
            return component;
        }

        public T Get<T>() where T : Component => (T)Get(typeof(T));

        public void Update(float deltaTime)
        {
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
                foreach (int entity in removeList)
                {
                    component.RemoveEntity(entity);
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